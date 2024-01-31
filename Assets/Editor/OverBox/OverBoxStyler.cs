using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public static class OverBoxStyler
{
    public static void SetTextFieldStyle(VisualElement visualElement)
    {
        SetBorder(visualElement, OvergrownColors.GetBorderColor(), 0.6f, 4);
        visualElement.style.backgroundColor = Color.black;
        visualElement.style.opacity = 0.9f;
    }

    public static void SetBorder(VisualElement visualElement, Color borderColor, float borderWidth, float borderRadius)
    {
        visualElement.style.borderBottomColor = visualElement.style.borderLeftColor = visualElement.style.borderRightColor = visualElement.style.borderTopColor = borderColor;
        visualElement.style.borderBottomWidth = visualElement.style.borderLeftWidth = visualElement.style.borderRightWidth = visualElement.style.borderTopWidth = borderWidth;
        visualElement.style.borderTopLeftRadius = visualElement.style.borderTopRightRadius = visualElement.style.borderBottomLeftRadius = visualElement.style.borderBottomRightRadius = borderRadius;
    }

    public static void SetNodeTitleStyle(OverBoxNode newNode, int characterType)
    {
        Color color = (characterType == (int)CharacterType.Heather) ? OvergrownColors.GetHeatherColor() : OvergrownColors.GetDanielColor();

        newNode.titleContainer.style.backgroundColor = color;
        newNode.mainContainer.style.borderBottomColor = newNode.mainContainer.style.borderLeftColor = newNode.mainContainer.style.borderRightColor = newNode.mainContainer.style.borderTopColor = color;
    }

    public static EnumField GenerateCharacterEnumField(OverBoxNode newNode)
    {
        EnumField enumField = new(newNode.character);

        enumField.StretchToParentWidth();
        enumField.style.top = 5;
        enumField.style.left = 3;
        enumField.style.right = 50;
        enumField.style.height = 24;
        enumField.style.opacity = 0.85f;

        return enumField;
    }

    public static Box GenerateAllChoicesBox()
    {
        Box allChoicesBox = new();
        allChoicesBox.style.flexDirection = FlexDirection.Column;
        allChoicesBox.style.marginBottom = allChoicesBox.style.marginTop = allChoicesBox.style.marginLeft = allChoicesBox.style.marginRight = 3;
        allChoicesBox.style.borderBottomLeftRadius = allChoicesBox.style.borderBottomRightRadius = allChoicesBox.style.borderTopLeftRadius = allChoicesBox.style.borderTopRightRadius = 2;
        allChoicesBox.style.borderTopWidth = allChoicesBox.style.borderLeftWidth = allChoicesBox.style.borderBottomWidth = allChoicesBox.style.borderRightWidth = 0;
        allChoicesBox.style.backgroundColor = OvergrownColors.GetLightGrey();

        return allChoicesBox;
    }

    public static TextField GenerateBubbleLineTextField()
    {
        TextField bubbleLineTextField = new();
        bubbleLineTextField.style.width = 125;
        bubbleLineTextField.value = String.Empty;

        return bubbleLineTextField;
    }

    public static TextField GenerateLongLineTextField()
    {
        TextField longLineTextField = new();

        longLineTextField.style.left = 1;
        longLineTextField.value = String.Empty;
        longLineTextField.multiline = true;
        longLineTextField.style.maxWidth = 200;

        return longLineTextField;
    }
}
