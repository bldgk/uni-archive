' heater_control.vbs — Temperature control with 3 heaters
'
' Simulates a heating chamber with 3 independent heaters and natural cooling.
' Each heater has a different max temperature target:
'   Heater 1: heats up to 30°C
'   Heater 2: heats up to 42°C
'   Heater 3: heats up to 50°C
' Natural cooling brings temperature down when heaters are off.
'
' Registers:
'   Coil 0, offset 0   = ventilator (on/off)
'   Coil 0, offset 2-4 = heater 1, 2, 3 (on/off)
'   Holding 3, offset 0 = temperature (float, 2 words)

' Include: modbus_float.vbs (FloatFromRegister / FloatToRegister)

Const HEAT_RATE  = 1.023    ' degrees per cycle when heating
Const BOOST_RATE = 0.023    ' extra heating at low temps
Const COOL_RATE  = 1.0      ' degrees per cycle when cooling
Const AMBIENT    = 25.0     ' ambient temperature

offset = 0

' ── Read inputs ──
vent = GetRegisterValue(0, offset)
nag1 = GetRegisterValue(0, offset + 2)
nag2 = GetRegisterValue(0, offset + 3)
nag3 = GetRegisterValue(0, offset + 4)
temperature = FloatFromRegister(3, offset)

' Initialize on first run
If temperature = 0 Then temperature = AMBIENT + 0.001

active_heaters = nag1 + nag2 + nag3

' ── Natural cooling ──
' Temperature drops toward ambient when heaters can't sustain it
If active_heaters = 0 And temperature > AMBIENT Then
    temperature = temperature - COOL_RATE
End If
If active_heaters = 1 And temperature > 30 Then
    temperature = temperature - COOL_RATE
End If
If active_heaters = 2 And temperature > 42 Then
    temperature = temperature - COOL_RATE
End If

' ── Heater 1: target 30°C ──
If nag1 = 1 Then
    If temperature < 30 Then
        temperature = temperature + HEAT_RATE
    End If
    If temperature < 15 Then
        temperature = temperature + BOOST_RATE
    End If
End If

' ── Heater 2: target 42°C ──
If nag2 = 1 Then
    If temperature < 42 Then
        temperature = temperature + HEAT_RATE
    End If
    If temperature < 25 Then
        temperature = temperature + BOOST_RATE
    End If
End If

' ── Heater 3: target 50°C ──
If nag3 = 1 Then
    If temperature < 50 Then
        temperature = temperature + HEAT_RATE
    End If
    If temperature < 30 Then
        temperature = temperature + BOOST_RATE
    End If
End If

' Clamp to ambient minimum
If temperature < AMBIENT Then temperature = AMBIENT

' ── Write output ──
FloatToRegister temperature, 3, offset

AddDebugString "Temperature: " & CStr(temperature) & "°C"
AddDebugString "Heaters: " & CStr(nag1) & "/" & CStr(nag2) & "/" & CStr(nag3)
