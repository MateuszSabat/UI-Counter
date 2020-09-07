# UI Counter
[Unity] Tool for displaying custom-font numbers as image

## General Notes
UI Counter is a toolkit for Unity that proccesses given integer to sprite and displays it as UnityEngine.UI.Image.

Processing is based on ScriptibleObject [Font](https://github.com/MateuszSabat/UI-Counter/blob/master/Assets/UI_Counter/Scripts/Font.cs).

## Fonts
You may customize font it uses.

You can change:
* spacing beetwen digits
* wheather or not it has borders and how they should look like
* shape of digits

## Digit Textures Requirement
In current version digit textures are processed to arrays of 0 and 1 describing if pixel's alpha is greater than 0.5 or not.

Given that, nearly every texture will fit the toolkit. If it gave some wird artifacts, you sholud check if there aren't any lonely pixels of alpha greater than 0.5 in a digit texture.

## Unity Version
It was created on unity version 2019.4.1f1
