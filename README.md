
# Entity Component System Tools

A set of tools to be used with Unity's Entity Component System (ECS). Currently there is only one editor window with two pages, one for systems and one for entities. In the near future they will be moved into their own editor windows.

## Systems Page
Each world is listed with all the systems available in the current assembly. The system list is obtained using the same method that is used in default initialization.<sup>[Note 1](#note1)</sup> If a system exists for a world it is listed first and, systems that don't exist are listed after. Each can be updated, disposed, and recreated independently of each other.~~<sup>[Note 2](#note2)</sup>~~

### World
World's do not currently have any functionality exposed in the UI.<sup>[Note 3](#note3)</sup>

### System
Each system has the following functions available in the UI.
* **Create:** Creates the system in the world it is listed under.~~<sup>[Note 2](#note2)</sup>~~
* **Update:** Calls `Update()` for the system in the world it is listed for. This does not bypass the internal checks used to determine if a system should run or not.
* **Dispose:** Calls `Dispose()` for the system in the world it is listed for.

## Entities Page
The Entities Page is split into two panels, the Components Panel is on the left and the Entities Panel is on the right.

### Components Panel
This is a list of all components available in the `TypeManager`. Components are sorted into 6 categories Shared, Shared System State, System State, Tag or Zero Sized, Buffer Array,<sup>[Note 4](#note4)</sup> and Components (IComponentData). Any component that implements multiple interfaces will be listed in multiple categories.<sup>[Note 5](#note5)</sup>

### Entities Panel
This is a list of all entities in the selected world. Entities are listed in the same order as clicking EntityManager in the Entity Debugger window and it shows the same readonly inspector when an entity is clicked. Using the foldout arrow displays a list of the components attached to that entity. Selecting a component will display editors the same way as selecting it in the components panel. **Components don't automatically update.** To update a component you **must** use the Set Component button along the bottom.<sup>[Note 6](#note6)</sup>

#### Editing Components
There are currently two different ways to edit a component. Selecting a component displays editors for that component's fields in the bottom panel. Clicking the foldout arrow displays a list of editors for that components fields. **Currently this only supports the different int and float types from Unity.Mathematics, and primitive types.** <sup>[Note 7](#note7)</sup>

#### Entity Manager Functions
The entity used for these functions is the entity shown in the inspector or it defaults to the last selected entity from any Entities panel. This allows you to select an entity from the Entity Debugger and use it.
- **Add Component** => Adds the selected component to the selected entity.
- **Set Component** => Sets the selected component on the selected entity.
- **Remove Component** => Removes the selected component from the selected entity.
- **Destroy Entity** => Destroys the selected entity;
- **Create Entity With Components** => Creates an entity with the selected components from the components panel.<sup>[Note 8](#note8)</sup>

<font size="4">
<b>Notes:</b>
</font><br/>
Limitations and plans for the future.<br/>

<font size="3">
<a name="note1"><b>Note 1:</b></a> There is limited support for systems that have a generic type as part of the class definition. The class will only display if it is part of the player loop. They can still be updated and disposed, but not created from the editor window.<br/>
<a name="note2"><b>Note 2:</b></a> <s>The ability to add multiple instances of a system to a world is planned for the future.</s> Not possible only one system can exists per world.<br/>
<a name="note3"><b>Note 3:</b></a> An update all systems and dispose are currently planned for the near future.<br/>
<a name="note4"><b>Note 4:</b></a> Buffer Array Components are not supported yet.<br/>
<a name="note5"><b>Note 5:</b></a> An option to change this maybe added later.<br/>
<a name="note6"><b>Note 6:</b></a> The plan is to make this set uneeded and automatically update the component.<br/>
<a name="note7"><b>Note 7:</b></a> I'm looking for a better way of displaying editors other than manually checking the type and displaying an editor.<br/>
<a name="note8"><b>Note 8:</b></a> Multi-Select is current disabled but it is planned for in the future.<br/>
</font>