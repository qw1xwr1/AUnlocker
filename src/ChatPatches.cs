[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
public static class AllowRussianOM_TextBoxTMP_IsCharAllowed_Prefix
{
    public static bool Prefix(TextBoxTMP __instance, ref bool __result, char i)
    {
        // Жёстко разрешаем русские буквы 'о' и 'м' (маленькие и заглавные)
        if (i == 'о' || i == 'м' || i == 'О' || i == 'М')
        {
            __result = true;
            return false; // пропускаем оригинал, разрешаем символ
        }

        // Остальной код из оригинала (без проверки AllowAllCharacters)
        if (i is >= 'À' and <= 'ÿ')
        {
            __result = true;
            return false;
        }
        if (i is >= 'Ѐ' and <= 'џ')
        {
            __result = true;
            return false;
        }
        if (i is >= '\u3040' and <= '㆟')
        {
            __result = true;
            return false;
        }
        if (i is >= 'ⱡ' and <= '힣')
        {
            __result = true;
            return false;
        }
        if (TextBoxTMP.SymbolChars.Contains(i))
        {
            __result = true;
            return false;
        }
        if (TextBoxTMP.EmailChars.Contains(i))
        {
            __result = true;
            return false;
        }
        if (i is '\b' or '\n' or '\r')
        {
            __result = false;
            return false;
        }

        // По умолчанию разрешаем все символы, кроме тех что явно запрещены выше
        __result = true;
        return false;
    }
}
