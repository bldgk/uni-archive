' modbus_float.vbs — IEEE 754 float <-> Modbus register conversion
'
' Modbus holds 16-bit words. A 32-bit float occupies 2 consecutive registers.
' These routines manually pack/unpack IEEE 754 single-precision floats
' since VBScript has no native bit-level float access.
'
' Usage:
'   temperature = FloatFromRegister(3, 0)   ' read float from holding reg 3, offset 0
'   FloatToRegister 25.5, 3, 0              ' write float to holding reg 3, offset 0

Dim HiHi_Byte, HiLo_Byte, LoHi_Byte, LoLo_Byte
Dim Hi_Word, Lo_Word

Function LongToFloat(X)
    Dim S, E, F
    S = -(X < 0)
    E = (X And &H7F800000) \ &H800000
    F = X And &H7FFFFF
    If (0 < E) And (E < 255) Then
        LongToFloat = (-1) ^ S * 2 ^ (E - 127) * ((F Or &H800000) / &H800000)
    ElseIf E = 0 Then
        If F = 0 Then
            LongToFloat = 0
        Else
            LongToFloat = (-1) ^ S * 2 ^ (-126) * (F / &H800000)
        End If
    Else ' E = 255 (Inf/NaN)
        LongToFloat = 0
    End If
End Function

Function TwoWordsToLong(W0, W1)
    If W0 < 0 Then W0 = 65535 + W0 + 1
    If W1 < 0 Then W1 = 65535 + W1 + 1
    If W1 <= &H7FFF Then
        TwoWordsToLong = W1 * &H10000 + W0
    Else
        TwoWordsToLong = -((65535 - W1) * &H10000 + (65535 - W0)) - 1
    End If
End Function

Sub FloatTo4Bytes(Y)
    Dim Sign, Exponent, Mantissa, multiply, m, C, firstbit
    If Y < 0 Then Sign = -1 Else Sign = 1
    If Sign = 1 Then firstbit = 0 Else firstbit = 1
    If Y = 0 Then
        HiHi_Byte = 0 : HiLo_Byte = 0
        LoHi_Byte = 0 : LoLo_Byte = 0
        Exit Sub
    Else
        Exponent = Int(Log(Abs(Y)) / Log(2))
    End If
    Mantissa = Y / 2 ^ Exponent / Sign
    multiply = (Mantissa - 1) * 8388608
    m = multiply / 256
    LoLo_Byte = Int((m - Int(m)) * 256 + 0.5)
    LoHi_Byte = (Int(m) / 256 - Int(Int(m) / 256)) * 256
    C = Int(Int(m) / 256)
    Exponent = Exponent + 127
    HiLo_Byte = (Exponent And &H1) * 128 + C
    HiHi_Byte = &H00
    If firstbit = 1 Then HiHi_Byte = &H80
    HiHi_Byte = HiHi_Byte + Int(Exponent / 2)
End Sub

Function FloatFromRegister(Reg, Offset)
    Hi_Word = GetRegisterValue(Reg, Offset + 1)
    Lo_Word = GetRegisterValue(Reg, Offset + 0)
    FloatFromRegister = LongToFloat(TwoWordsToLong(Lo_Word, Hi_Word))
End Function

Sub FloatToRegister(Value, Reg, Offset)
    FloatTo4Bytes Value
    Lo_Word = LoHi_Byte * 256 + LoLo_Byte
    Hi_Word = HiHi_Byte * 256 + HiLo_Byte
    SetRegisterValue Reg, Offset + 1, Hi_Word
    SetRegisterValue Reg, Offset + 0, Lo_Word
End Sub
