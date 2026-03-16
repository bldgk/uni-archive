;;
;; enableA20.s (adapted from Visopsys OS-loader)
;;
;; Copyright (c) 2000, J. Andrew McLaughlin
;; You're free to use this code in any manner you like, as
;; long as this notice is included (and you give credit where
;; it is due), and as long as you understand and accept that
;; it comes with NO WARRANTY OF ANY KIND.
;; Contact me at jamesamc@yahoo.com about any bugs or problems.
;;

enableA20:
;; Enable A20 address line via the keyboard controller.
;; Arguments: none.  Returns: 0 in EAX on success,
;; -1 on failure.  Written for use in 16-bit code
;; (32-bit sections marked as 32-BIT).

pusha

;; Make sure interrupts are disabled
cli

;; CX will be our retry counter (up to 5 attempts to enable A20)
mov CX, 5

.startAttempt1:
;; Wait until the controller is ready to accept commands
.commandWait1:
xor AX, AX
in AL, 64h
bt AX, 1
jc .commandWait1

;; Tell the controller we want to read the current status
;; (command D0h)
mov AL, 0D0h
out 64h, AL

;; Wait until the controller is ready
.dataWait1:
xor AX, AX
in AL, 64h
bt AX, 0
jnc .dataWait1

;; Read the current status from port 60h
xor AX, AX
in AL, 60h

;; Save the current value of (E)AX
push AX; 16-BIT
;; push EAX; 32-BIT

;; Wait until the controller is ready
.commandWait2:
in AL, 64h
bt AX, 1
jc .commandWait2

;; Tell the controller we want to write the status byte again
mov AL, 0D1h
out 64h, AL

;; Wait until the controller is ready
.commandWait3:
xor AX, AX
in AL, 64h
bt AX, 1
jc .commandWait3

;; Write the new value to port 60h (previous value was saved on stack)
pop AX; 16-BIT
;; pop EAX; 32-BIT

;; Set the A20 enable bit
or AL, 00000010b
out 60h, AL

;; Now read back the status byte to verify A20 was enabled

;; Wait until the controller is ready
.commandWait4:
xor AX, AX
in AL, 64h
bt AX, 1
jc .commandWait4

;; Command D0h
mov AL, 0D0h
out 64h, AL

;; Wait until the controller is ready
.dataWait2:
xor AX, AX
in AL, 64h
bt AX, 0
jnc .dataWait2

;; Read the status from port 60h
xor AX, AX
in AL, 60h

;; Is the A20 gate enabled?
bt AX, 1

;; If CF is set, A20 is enabled
jc .success

;; Retry if CX hasn't reached zero
loop .startAttempt1


;; The standard method failed.
;; Now try the alternate method (not supported on all chipsets,
;; but on some it's the only way)


;; Retry counter
mov CX, 5

.startAttempt2:
;; Wait until the controller is ready
.commandWait6:
xor AX, AX
in AL, 64h
bt AX, 1
jc .commandWait6

;; Tell the controller to enable A20
mov AL, 0DFh
out 64h, AL

;; Read back A20 status to confirm it's enabled

;; Wait until the controller is ready
.commandWait7:
xor AX, AX
in AL, 64h
bt AX, 1
jc .commandWait7

;; Command D0h
mov AL, 0D0h
out 64h, AL

;; Wait until the controller is ready
.dataWait3:
xor AX, AX
in AL, 64h
bt AX, 0
jnc .dataWait3

;; Read the status byte from port 60h
xor AX, AX
in AL, 60h

;; Is A20 enabled?
bt AX, 1

;; If CF is set, A20 is enabled.
;; Warn that we had to use the alternate method
jc .warn

;; Retry if CX hasn't reached zero
loop .startAttempt2


;; Both methods failed. An error message could go here.
jmp .fail


.warn:
;; A warning message about using the alternate method could go here

.success:
sti
popa
xor EAX, EAX
ret

.fail:
sti
popa
mov EAX, -1
ret
