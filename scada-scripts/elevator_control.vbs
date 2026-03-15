' elevator_control.vbs — Grain elevator with 2 tanks
'
' Simulates a grain processing facility with:
'   Tank 1: filled via valve (q1_in_val), drained by elevator (noria)
'   Tank 2: filled by elevator, drained by trieur
'   Both tanks have level sensors and overflow detection
'
' Physical model:
'   Level = Volume / (pi * r^2)
'   Volume changes by flow_rate * dt per cycle
'
' Registers:
'   Holding 3, offset 0  = tank 1 level (float)
'   Holding 3, offset 2  = valve 1 opening (float, 0.0-1.0)
'   Holding 3, offset 4  = crusher valve opening (float)
'   Holding 3, offset 6  = tank 1 fill ratio (float)
'   Holding 3, offset 8  = tank 1 overflow flag (float, 0/1)
'   Holding 3, offset 10 = tank 2 level (float)
'   Holding 3, offset 12 = valve 2 opening (float, 0.0-1.0)
'   Holding 3, offset 14 = trieur valve opening (float)
'   Holding 3, offset 16 = tank 2 overflow flag (float, 0/1)

' Include: modbus_float.vbs

Const PI = 3.1415
Const DT = 0.1              ' time step

' Tank parameters
Const R_TANK1  = 20          ' radius (m)
Const R_TANK2  = 15
Const Q_MAX1   = 20000       ' max volume (m^3)
Const Q_MAX2   = 15000
Const Q1_INFLOW = 800        ' inflow rate for valve 1
Const NORIA_SPEED = 1500     ' elevator transfer speed

Dim q1, q2
Dim le1, le2
Dim le1_max, le2_max
Dim overflow1, overflow2

offset = 0

' ── Read inputs ──
le1           = FloatFromRegister(3, offset + 0)
valve_in_nor1 = FloatFromRegister(3, offset + 2)
valve_in_drob = FloatFromRegister(3, offset + 4)

le2           = FloatFromRegister(3, offset + 10)
valve_in_nor2 = FloatFromRegister(3, offset + 12)
valve_in_trier = FloatFromRegister(3, offset + 14)

' ── Calculate max levels ──
le1_max = Q_MAX1 / (PI * R_TANK1 * R_TANK1)
le2_max = Q_MAX2 / (PI * R_TANK2 * R_TANK2)

' ── Update volumes ──
q1 = le1 * (PI * R_TANK1 * R_TANK1)   ' recover volume from level
q2 = le2 * (PI * R_TANK2 * R_TANK2)

q1 = q1 + DT * Q1_INFLOW * valve_in_nor1
q2 = q2 + DT * NORIA_SPEED * valve_in_nor2

' ── Update levels ──
le1 = q1 / (PI * R_TANK1 * R_TANK1)
le2 = q2 / (PI * R_TANK2 * R_TANK2)

' ── Fill ratio for monitoring ──
nor_plot_koef_1 = le1 / le1_max

' ── Tank 1 bounds check ──
overflow1 = 0
If q1 <= 0.1 Then
    q1 = 0 : le1 = 0
End If
If q1 > Q_MAX1 Then
    q1 = Q_MAX1
    le1 = le1_max
    overflow1 = 1
End If

' ── Tank 2 bounds check ──
overflow2 = 0
If q2 <= 0.1 Then
    q2 = 0 : le2 = 0
End If
If q2 > Q_MAX2 Then
    q2 = Q_MAX2
    le2 = le2_max
    overflow2 = 1
End If

' ── Write outputs ──
FloatToRegister le1, 3, offset
FloatToRegister valve_in_nor1, 3, offset + 2
FloatToRegister nor_plot_koef_1, 3, offset + 6
FloatToRegister overflow1, 3, offset + 8
FloatToRegister le2, 3, offset + 10
FloatToRegister overflow2, 3, offset + 16

AddDebugString "Tank 1 level: " & CStr(Round(le1, 2)) & " m (" & CStr(Round(nor_plot_koef_1 * 100, 1)) & "%)"
AddDebugString "Tank 2 level: " & CStr(Round(le2, 2)) & " m"
If overflow1 = 1 Then AddDebugString "WARNING: Tank 1 overflow!"
If overflow2 = 1 Then AddDebugString "WARNING: Tank 2 overflow!"
