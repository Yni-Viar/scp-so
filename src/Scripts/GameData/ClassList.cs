using Godot;
using System;

public partial class ClassList : Resource
{
    [Export]
    public Godot.Collections.Array<BaseClass> SingleSpawnClass { get; set; }

    [Export]
    public Godot.Collections.Array<BaseClass> MultipleSpawnClass { get; set; }

    [Export]
    public Godot.Collections.Array<BaseClass> ArrivingClass { get; set; }

    [Export]
    public Godot.Collections.Array<BaseClass> SpecialClass { get; set; }

    public ClassList() : this(new Godot.Collections.Array<BaseClass>(), new Godot.Collections.Array<BaseClass>(),
        new Godot.Collections.Array<BaseClass>(), new Godot.Collections.Array<BaseClass>())
    { }

    public ClassList(Godot.Collections.Array<BaseClass> singleSpawnClass, Godot.Collections.Array<BaseClass> multipleSpawnClass,
        Godot.Collections.Array<BaseClass> arrivingClass, Godot.Collections.Array<BaseClass> specialClass)
    {
        SingleSpawnClass = singleSpawnClass;
        MultipleSpawnClass = multipleSpawnClass;
        ArrivingClass = arrivingClass;
        SpecialClass = specialClass;
    }
}
