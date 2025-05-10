# FormatStrings

A formatted string, for displaying colored, bold, underlined, etc. text in the console using ANSI escape sequences.  
It also provides many useful functionalities for string and formatting purposes 

## CharFormat
It uses the CharFormat class, that has options for bold, thin, italic, underline, double underline, strike-through, foreground color and background color.

Check the [Reference Documentation](./AshLibReferenceDocumentation.pdf) for information on how to use it.

## Appending

Lets start by creating a formattted string
```cs
FormatString fs = new FormatString();
```

And appending:
```cs
fs.Append("hello", new CharFormat(Color3.Red));
```

This way, we added a red word. Now lets add another in a different color:
```cs
fs.Append(" world", new CharFormat(Color3.Blue));
```

And if we do:
```cs
Console.WriteLine(fs);
```
We should see the colors.

## String formatting

You can format strings with charachters themselves. Formatting applies to carachters after the definition until the formatting is changed.  
Options are enclosed in /\[ and ], and separated by commas
Options:
|Symbol|Additional Params|Description|
|--:|:--|---|
|B| |Bold text|
|T| |Thin text|
|D| |Normal density text|
|RT| |Normal density text|
|RD| |Normal density text|
|I| |Italic text|
|RI| |Not italic text|
|U| |Underlined text|
|DU| |Double underlined text|
|RU| |Not underlined text|
|S| |Strike-through text|
|RS| |No strike-through text|
|C|R,G,B|Text color (RGB)|
|F|R,G,B|Text color (RGB)|
|C#|HexColor|Text color (hex)|
|F#|HexColor|Text color (hex)|
|RC| |Reset text color to terminal default|
|RF| |Reset text color to terminal default|
|BG|R,G,B|Background color (RGB)|
|BG#|HexColor|Background color (hex)|
|RB| |Reset background color to terminal default|
|0| |Reset all properties to terminal default|

These can be combines, separated by commas:
`/[B,U]Hello!`
Bold and underlined at the same time
