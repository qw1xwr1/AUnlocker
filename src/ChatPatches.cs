using HarmonyLib;
using UnityEngine;

namespace AUnlocker;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public static class ChatJailbreak_ChatController_Update_Postfix
{
    public static void Postfix(ChatController __instance)
    {
        if (AUnlocker.NoChatCooldown.Value)
        {
            __instance.timeSinceLastMessage = 3f;
        }

        if (AUnlocker.NoCharacterLimit.Value)
        {
            __instance.freeChatField.textArea.characterLimit = int.MaxValue;
        }

        else if (AUnlocker.PatchChat.Value)
        {
            __instance.freeChatField.textArea.AllowPaste = true;
            __instance.freeChatField.textArea.AllowSymbols = true;
            __instance.freeChatField.textArea.AllowEmail = true;
            __instance.freeChatField.textArea.allowAllCharacters = true;
            __instance.freeChatField.textArea.characterLimit = 120;
        }
    }
}

[HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.UpdateCharCount))]
public static class EditColorIndicators_FreeChatInputField_UpdateCharCount_Postfix
{
    public static void Postfix(FreeChatInputField __instance)
    {
        if (AUnlocker.NoCharacterLimit.Value)
        {
            var length = __instance.textArea.text.Length;
            __instance.charCountText.SetText($"{length}/{__instance.textArea.characterLimit}");
            __instance.charCountText.color = length switch
            {
                < 1610612735 => Color.black,
                < 2147483647 => new Color(1f, 1f, 0f, 1f),
                _ => Color.red
            };
        }

        else if (AUnlocker.PatchChat.Value)
        {
            var length = __instance.textArea.text.Length;
            __instance.charCountText.SetText($"{length}/{__instance.textArea.characterLimit}");
            __instance.charCountText.color = length switch
            {
                < 90 => Color.black,
                < 120 => new Color(1f, 1f, 0f, 1f),
                _ => Color.red
            };
        }
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendFreeChat))]
public static class AllowURLS_ChatController_SendFreeChat_Prefix
{
    public static bool Prefix(ChatController __instance)
    {
        if (!AUnlocker.PatchChat.Value) return true;

        var text = __instance.freeChatField.Text;
        ChatController.Logger.Debug($"SendFreeChat() :: Sending message: '{text}'");
        PlayerControl.LocalPlayer.RpcSendChat(text);
        return false;
    }
}

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
public static class AllowAllCharacters_TextBoxTMP_IsCharAllowed_Prefix
{
    public static bool Prefix(TextBoxTMP __instance, ref bool __result, char i)
    {
        if (!AUnlocker.AllowAllCharacters.Value) return true;

        // просто разрешаем любой символ
        __result = true;
        return false;
    }
}

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.Start))]
public static class AllowPaste_TextBoxTMP_Start_Postfix
{
    public static void Postfix(TextBoxTMP __instance)
    {
        if (!AUnlocker.PatchChat.Value) return;

        __instance.allowAllCharacters = true;
        __instance.AllowEmail = true;
        __instance.AllowPaste = true;
        __instance.AllowSymbols = true;
    }
}

[HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.Update))]
public static class AllowCopy_TextBoxTMP_Update_Postfix
{
    public static void Postfix(TextBoxTMP __instance)
    {
        if (!AUnlocker.PatchChat.Value || !__instance.hasFocus) return;

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
        {
            ClipboardHelper.PutClipboardString(__instance.text);
        }
    }
}

