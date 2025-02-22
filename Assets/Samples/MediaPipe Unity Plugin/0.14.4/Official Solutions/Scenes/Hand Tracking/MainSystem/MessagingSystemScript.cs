using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Drawing;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;
using Color = UnityEngine.Color;

public class MessagingSystemScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI MessagingLine;
    [SerializeField] RawImage BackgroundPane;

    float alpha = 1;
    UnityEngine.Color TextColor;
    UnityEngine.Color BackgroundColor;
    // Start is called before the first frame update
    void Start()
    {
        SetMessage("Добро пожаловать!", Color.green);
    }

    public void SendBlackMessage(string _Message)
    {
        SetMessage(_Message, Color.black);
    }

    public void SetMessage(string _Message, UnityEngine.Color _Color)
    {
        CancelInvoke("FadeOut");
        MessagingLine.text = _Message;
        TextColor = new UnityEngine.Color(_Color.r, _Color.g, _Color.b);
        BackgroundColor = new UnityEngine.Color(BackgroundPane.color.r, BackgroundPane.color.g, BackgroundPane.color.b);
        InvokeRepeating("FadeIn", 1, 0.1f);
        InvokeRepeating("FadeOut", 6, 0.1f);
    }

    public void FadeIn()
    {
        alpha += 0.2f;
        TextColor.a = alpha;
        BackgroundColor.a = alpha;
        MessagingLine.color = TextColor;
        BackgroundPane.color = BackgroundColor;
        if (alpha >= 1) CancelInvoke("FadeIn");
    }

    public void FadeOut()
    {
        TextColor = new UnityEngine.Color(MessagingLine.color.r, MessagingLine.color.g, MessagingLine.color.b);
        BackgroundColor = new UnityEngine.Color(BackgroundPane.color.r, BackgroundPane.color.g, BackgroundPane.color.b);
        alpha -= 0.1f;
        TextColor.a = alpha;
        BackgroundColor.a = alpha;
        MessagingLine.color = TextColor;
        BackgroundPane.color = BackgroundColor;
        if (alpha <= 0) CancelInvoke("FadeOut");
    }
}