' furnace_control.vbs — Metallurgical furnace + rolling + cooling
'
' Simulates a two-zone industrial process:
'   Zone 1 (Furnace): heated by burner (nagr) and pump, temperature rises
'   Zone 2 (Cooling): receives hot material, cooled by rolling mill and cooler
'
' When furnace is off and hot enough, material transfers to cooling zone.
' Both zones cool toward ambient (24°C) naturally.
'
' Registers:
'   Coil 0, offset 0 = burner (on/off)
'   Coil 0, offset 1 = pump (on/off)
'   Coil 0, offset 2 = rolling mill (on/off)
'   Coil 0, offset 3 = cooler (on/off)
'   Holding 3, offset 0 = furnace temperature (float)
'   Holding 3, offset 2 = cooling room temperature (float)

' Include: modbus_float.vbs

Const DT_BURNER  = 20.23    ' heating rate from burner
Const DT_PUMP    = 5.47     ' heating rate from pump
Const DT_COOLER  = 23.89    ' cooling rate from cooler
Const DT_ROLLING = 13.89    ' cooling rate from rolling
Const AMBIENT    = 24.0

Function StatusStr(name, state)
    If state = 1 Then
        StatusStr = name & " is ON"
    Else
        StatusStr = name & " is OFF"
    End If
End Function

offset = 0

' ── Read inputs ──
nagr    = GetRegisterValue(0, offset)
pump    = GetRegisterValue(0, offset + 1)
rolling = GetRegisterValue(0, offset + 2)
cooling = GetRegisterValue(0, offset + 3)

te1 = FloatFromRegister(3, offset + 0)  ' furnace temp
te2 = FloatFromRegister(3, offset + 2)  ' cooling room temp

AddDebugString StatusStr("Burner", nagr)
AddDebugString StatusStr("Pump", pump)
AddDebugString StatusStr("Rolling", rolling)
AddDebugString StatusStr("Cooler", cooling)

' ── Zone 1: Furnace ──
If nagr = 1 Then
    te1 = te1 + DT_BURNER
Else
    te1 = te1 - DT_BURNER / 3   ' slow natural cooling
End If

If pump = 1 Then
    te1 = te1 + DT_PUMP
Else
    te1 = te1 - DT_PUMP / 3
End If

' ── Transfer: furnace → cooling room ──
If nagr = 0 And pump = 0 And te1 > AMBIENT Then
    te2 = te1
End If

' ── Zone 2: Cooling ──
If rolling = 1 Then
    te2 = te2 - DT_ROLLING
End If

If cooling = 1 Then
    te2 = te2 - DT_COOLER
End If

' ── Clamp to ambient ──
If te1 < AMBIENT Then te1 = AMBIENT
If te2 < AMBIENT Then te2 = AMBIENT

' ── Write outputs ──
FloatToRegister te1, 3, offset + 0
FloatToRegister te2, 3, offset + 2

AddDebugString "Furnace: " & CStr(Int(te1)) & "°C"
AddDebugString "Cooling: " & CStr(Int(te2)) & "°C"
