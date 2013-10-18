﻿module Nu.Group
open System
open FSharpx
open FSharpx.Lens.Operators
open Nu.Core
open Nu.Entity

/// A game entity group.
/// A serializable value type.
type [<StructuralEquality; NoComparison>] Group =
    { Id : Id
      IsEnabled : bool
      IsVisible : bool
      Entities : Map<Lun, Entity> }
    with
        static member private optChildFinder addressHead parent =
            Map.tryFind addressHead parent.Entities
        
        static member private childFinder addressHead parent =
            let optChild = Group.optChildFinder addressHead parent
            match optChild with
            | None -> failwith ("Could not find child at address '" + str addressHead + "'.")
            | Some child -> child
        
        static member private childAdder addressHead parent child =
            { parent with Entities = Map.add addressHead child parent.Entities }
        
        static member private childRemover addressHead parent =
            { parent with Entities = Map.remove addressHead parent.Entities }
        
        static member private childGuiSetter child gui =
            { child with EntitySemantic = Gui gui }
        
        static member private childToGui child =
            match child.EntitySemantic with
            | Gui gui -> gui
            | _ -> failwith "Semantic of wrong type."
        
        static member private childToOptGui child =
            match child.EntitySemantic with
            | Gui gui -> Some gui
            | _ -> None
        
        static member private childGuiButtonSetter child gui button =
            let gui2 = { gui with GuiSemantic = Button button }
            { child with EntitySemantic = Gui gui2 }
        
        static member private childToGuiButton child =
            match child.EntitySemantic with
            | Gui gui ->
                match gui.GuiSemantic with
                | Button button -> (gui, button)
                | _ -> failwith "Semantic of wrong type."
            | _ -> failwith "Semantic of wrong type."
        
        static member private childToOptGuiButton child =
            match child.EntitySemantic with
            | Gui gui ->
                match gui.GuiSemantic with
                | Button button -> Some (gui, button)
                | _ -> None
            | _ -> None
        
        static member private childGuiLabelSetter child gui label =
            let gui2 = { gui with GuiSemantic = Label label }
            { child with EntitySemantic = Gui gui2 }
        
        static member private childToGuiLabel child =
            match child.EntitySemantic with
            | Gui gui ->
                match gui.GuiSemantic with
                | Label label -> (gui, label)
                | _ -> failwith "Semantic of wrong type."
            | _ -> failwith "Semantic of wrong type."
        
        static member private childToOptGuiLabel child =
            match child.EntitySemantic with
            | Gui gui ->
                match gui.GuiSemantic with
                | Label label -> Some (gui, label)
                | _ -> None
            | _ -> None
        
        static member private childActorSetter child actor =
            { child with EntitySemantic = Actor actor }
        
        static member private childToActor child =
            match child.EntitySemantic with
            | Actor actor -> actor
            | _ -> failwith "Semantic of wrong type."
        
        static member private childToOptActor child =
            match child.EntitySemantic with
            | Actor actor -> Some actor
            | _ -> None
        
        static member private childActorBlockSetter child actor block =
            let actor2 = { actor with ActorSemantic = Block block }
            { child with EntitySemantic = Actor actor2 }
        
        static member private childToActorBlock child =
            match child.EntitySemantic with
            | Actor actor ->
                match actor.ActorSemantic with
                | Block block -> (actor, block)
                | _ -> failwith "Semantic of wrong type."
            | _ -> failwith "Semantic of wrong type."
        
        static member private childToOptActorBlock child =
            match child.EntitySemantic with
            | Actor actor ->
                match actor.ActorSemantic with
                | Block block -> Some (actor, block)
                | _ -> None
            | _ -> None
        
        static member entity address =
            { Get = fun this -> getChild Group.childFinder this address
              Set = fun entity this -> setChild Group.childAdder this address entity }
        
        static member optEntity address =
            { Get = fun this -> getOptChild Group.optChildFinder this address
              Set = fun optEntity this -> setOptChild Group.childAdder Group.childRemover this address optEntity }
        
        static member entityGui address =
            { Get = fun this -> getChildSem Group.childFinder Group.childToGui address this
              Set = fun (entity, gui) this -> setChildSem Group.childAdder Group.childGuiSetter address this entity gui }
        
        static member optEntityGui address =
            { Get = fun this -> getOptChildSem Group.optChildFinder Group.childToOptGui this address
              Set = fun optEntityGui this -> setOptChildSem Group.childAdder Group.childRemover Group.childGuiSetter optEntityGui this address }
        
        static member entityGuiButton address =
            { Get = fun this -> getChildSemSem Group.childFinder Group.childToGuiButton address this
              Set = fun (entity, gui, button) this -> setChildSemSem Group.childAdder Group.childGuiButtonSetter address this entity gui button }
        
        static member optEntityGuiButton address =
            { Get = fun this -> getOptChildSemSem Group.optChildFinder Group.childToOptGuiButton this address
              Set = fun optEntityGuiButton this -> setOptChildSemSem Group.childAdder Group.childRemover Group.childGuiButtonSetter optEntityGuiButton this address }
        
        static member entityGuiLabel address =
            { Get = fun this -> getChildSemSem Group.childFinder Group.childToGuiLabel address this
              Set = fun (entity, gui, label) this -> setChildSemSem Group.childAdder Group.childGuiLabelSetter address this entity gui label }
        
        static member optEntityGuiLabel address =
            { Get = fun this -> getOptChildSemSem Group.optChildFinder Group.childToOptGuiLabel this address
              Set = fun optEntityGuiLabel this -> setOptChildSemSem Group.childAdder Group.childRemover Group.childGuiLabelSetter optEntityGuiLabel this address }

        static member entityActor address =
            { Get = fun this -> getChildSem Group.childFinder Group.childToActor address this
              Set = fun (entity, actor) this -> setChildSem Group.childAdder Group.childActorSetter address this entity actor }
        
        static member optEntityActor address =
            { Get = fun this -> getOptChildSem Group.optChildFinder Group.childToOptActor this address
              Set = fun optEntityActor this -> setOptChildSem Group.childAdder Group.childRemover Group.childActorSetter optEntityActor this address }
        
        static member entityActorBlock address =
            { Get = fun this -> getChildSemSem Group.childFinder Group.childToActorBlock address this
              Set = fun (entity, actor, block) this -> setChildSemSem Group.childAdder Group.childActorBlockSetter address this entity actor block }
        
        static member optEntityActorBlock address =
            { Get = fun this -> getOptChildSemSem Group.optChildFinder Group.childToOptActorBlock this address
              Set = fun optEntityActorBlock this -> setOptChildSemSem Group.childAdder Group.childRemover Group.childActorBlockSetter optEntityActorBlock this address }
        
        static member entities =
            { Get = fun this -> this.Entities
              Set = fun entities this -> { this with Entities = entities }}