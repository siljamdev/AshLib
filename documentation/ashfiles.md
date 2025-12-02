# AshFiles

An AshFile is a file format that has **.ash extension**, but it also represents a structure of data.  
This structure is based in multiple pockets of information. These pockets are called **camps**.  
Each camp has a **key** and a **value**. This value can be many things.  
For example, you could have a camp with key "debt", with its value being a number and being 12000.  

## Camp management
Start by creating an AshFile:
```cs
AshFile a = new AshFile();
```

Now we can add camps:
```cs
a.SetCamp("hello", "world");
a.SetCamp("num", -57.8);
```
There are many more options, check the [Reference Documentation](./AshLibReferenceDocumentation.pdf) or [Usage Documentation](./AshLibUsageDocumentation.pdf).  

## Byte format
AshFiles supprt being converted to a very compact byte format. The current version is V4.

You can easily do this like:
```cs
byte[] b = a.WriteToBytes();

//or directly save to file
a.Save("file.ash");
```

There are many options for this byte format, like the format itself (V1, V2, V3, V4), `compactBools`, `maskCampNames`, `maskStrings` and an optional `password`.

## String format
There is also a somewhat readable string format:
`<name> : type : value ;`  
That is a single camp, after the semicolon there can be more. Also, it can be arrays:
`<name> : type : [value1; value2; value3];`  
Any amount of whitespace is okay.

List of supported types:
|Symbol|Type|Value specifications|
|--:|:--|---|
|@|string|values must be enclosed with ""|
|ub|byte| |
|us|ushort| |
|ui|uint| |
|ul|ulong| |
|sb|sbyte| |
|s|short| |
|n|int| |
|i|int| |
|l|long| |
|#|Color3|struct defined by AshLib, values must be in hex|
|f|float| |
|d|double| |
|v2|Vec2|struct defined by AshLib, 2 float values are expected serparated by commas|
|v3|Vec3|struct defined by AshLib, 3 float values are expected serparated by commas|
|v4|Vec4|struct defined by AshLib, 4 float values are expected serparated by commas|
|b|boolean| |
|dt|Date|struct defined by AshLib, 6 numbers are expected separated by /, day, month, year, hour, minute, second|

You can get it with:
```cs
string s = a.ToString();

//or the compact form
string s = a.ToStringCompact();
```  

And parse(or try!) like:
```cs
AshLib a = AshLib.Parse(s);
```

Also, you can generate a JSON:
```cs
string json = a.ToJson();
```

## Visualizing
Usually, camp names are separated like forlders of a file system using dots (.). For example:  
`user.preferences.darkMode: true`

You can visualize a list of all camps using:
```cs
Console.WriteLine(a.Visualize());
```

Or visualize the tree form, using . as the separator:
```cs
Console.WriteLine(a.VisualizeAsTree());
```

Or even visualize with formatting, using [FormatStrings](./formatstrings.md)
```cs
Console.WriteLine(a.VisualizeAsFormattedTree(new CharFormat(Color3.Red), new CharFormat(Color3.Blue), new CharFormat(Color3.Green)));
```
This way, tree lines will be red, camp names blue and camp values green
