# ECS Tools
A collection of tools to be used with Unity's Entity Component System (ECS), currently there are only two pages one for systems and one for entities. This is likely to change as functionality is added.

## Systems Page
Each world is listed with all the systems available in the current assembly, the system list is obtained the same as in default initialization. If multiple instances of the same system exist each instance will be listed for that world and each can be updated, disposed, and recreated independently of each other. When no instances of a system exist only one is listed for that world to use.<sup>[Note 1](#note1)</sup>

### World
Worlds do not currently have any functionality exposed in the UI.<sup>[Note 2](#note2)</sup>

### System
Each system has the following functions available in the UI.
* Create: Creates the system in the world it is listed under.<sup>[Note 1](#note1)</sup>
* Update: Calls `Update()` for the system in the world it is listed for. This does not bypass the internal checks used to determine if a system should run or not.
* Dispose: Calls `Dispose()` for the system in the world it is listed for.

## Entities Page
The entities page is split in two panels on the left is Components panel and the right is the Entities Panel.

### Components Panel
This is a list of all components available in the `TypeManager`. Components are sorted into 6 categories Shared, Shared System State, System State, Tag or Zero Sized, Buffer Array<sup>[Note 3](#note3)</sup>, and Components (IComponentData). Any component that implements multiple interfaces will be listed in multiple categories.<sup>[Note 4](#note4)</sup>

### Entities Panel
This is a list of all entities in the selected world. Entities are listed in the same order as clicking EntityManager in the Entity Debugger window and it shows the same readonly inspector when an entity is clicked. Using the foldout arrow displays a list of the components attached to that entity. Selecting a component will display editors the same way as selecting it in the components panel. **Components don't automatically update.** To update a component you must use the Set Component button along the bottom.<sup>[Note 5](#note5)</sup>

#### Editing Components
There are currently two different ways to edit a component. Selecting a component displays editors for that component's fields in the bottom panel. Clicking the foldout arrow displays a list of editors for that components fields. **Currently this only supports the different int and float types from Unity.Mathematics, and primitive types.** <sup>[Note 6](#note6)</sup>

#### Entity Manager Functions
The entity used as for these functions is the entity shown in the inspector or it defaults to the last selected entity from the Entities panel. This allows you to select an entity from the Entity Debugger and use it.
- Add Component => Adds the selected component to the selected entity.
- Set Component => Sets the selected component on the selected entity.
- Remove Component => Removes the selected component from the selected entity.
- Destroy Entity => Destroys the selected entity;
- Create Entity With Components => Creates an entity with the selected components from the components panel.<sup>[Note 7](#note7)</sup>

#### Notes:
<font size="3">
<a name="note1"><b>Note 1:</b> The ability to add instances of a system to a world is planned for the future.                                    </a><br> 
<a name="note2"><b>Note 2:</b> An update all systems and dispose are currently planned for the near future.                                      </a><br> 
<a name="note3"><b>Note 3:</b> Buffer Array Components are not supported yet.                                                                    </a><br> 
<a name="note4"><b>Note 4:</b> An option to change this maybe added later.                                                                       </a><br> 
<a name="note5"><b>Note 5:</b> The plan is to make this set uneeded and automatically update the component.                                      </a><br>
<a name="note6"><b>Note 6:</b> I'm looking for a better way of displaying editors other than manually checking the type and displaying an editor.</a><br>
<a name="note7"><b>Note 7:</b> Multi-Select is current disabled but it is planned for in the future.                                             </a><br>
</font>