﻿// Nu Game Engine.
// Copyright (C) Bryan Edds, 2013-2020.

namespace OmniBlade
open System
open Prime
open Nu
open Nu.Declarative
open OmniBlade

[<AutoOpen>]
module RingMenuDispatcher =

    type [<StructuralEquality; NoComparison>] RingMenu =
        { Items : Map<int, string * bool>
          ItemCancelOpt : string option }

    type RingMenuCommand =
        | ItemCancel
        | ItemSelect of string
        | ArrangeItemButton of Entity * int

    type Entity with
        member this.GetRadius = this.Get Property? Radius
        member this.SetRadius = this.Set Property? Radius
        member this.Radius = lens<single> Property? Radius this.GetRadius this.SetRadius this
        member this.GetRingMenu = this.GetModel<RingMenu>
        member this.SetRingMenu = this.SetModel<RingMenu>
        member this.RingMenu = this.Model<RingMenu> ()
        member this.ItemSelectEvent = Events.ItemSelect --> this
        member this.CancelEvent = Events.Cancel --> this

    type RingMenuDispatcher () =
        inherit GuiDispatcher<RingMenu, unit, RingMenuCommand> ({ Items = Map.empty; ItemCancelOpt = None })

        static member Properties =
            [define Entity.Radius 84.0f
             define Entity.Rotation 0.0f
             define Entity.SwallowMouseLeft false
             define Entity.Visible false]

        override this.Command (ringMenu, command, menu, world) =
            match command with
            | ItemCancel -> just (World.publishPlus () menu.CancelEvent [] menu true world)
            | ItemSelect item -> just (World.publishPlus item menu.ItemSelectEvent [] menu true world)
            | ArrangeItemButton (button, index) ->
                let radius = menu.GetRadius world
                let itemCount = Map.count ringMenu.Items
                let progress = single index / single itemCount
                let rotation = progress * single Math.PI * 2.0f
                let position = v2 (radius * sin rotation) (radius * cos rotation)
                let world = button.SetPositionLocal (position - button.GetSize world * 0.5f) world
                just world

        override this.Content (ringMenu, menu) =
            [Content.entities ringMenu
                (fun ringMenu _ -> ringMenu.Items)
                (fun items _ -> items)
                (fun index itemNameAndEnabled world ->
                    let itemName = Lens.get itemNameAndEnabled world |> fst
                    let buttonName = menu.Name + "+" + scstring index
                    let button = menu.Parent / buttonName
                    Content.button button.Name
                        [Entity.EnabledLocal <== itemNameAndEnabled --> snd
                         Entity.Size == v2 48.0f 48.0f
                         Entity.Elevation <== menu.Elevation
                         Entity.UpImage == asset Assets.Battle.PackageName (itemName + "Up")
                         Entity.DownImage == asset Assets.Battle.PackageName (itemName + "Down")
                         Entity.ClickEvent ==> cmd (ItemSelect itemName)
                         Entity.UpdateEvent ==> cmd (ArrangeItemButton (button, index))])
             Content.entityOpt ringMenu (fun ringMenu _ -> ringMenu.ItemCancelOpt) $ fun itemCancel world ->
                let itemCancelValue = itemCancel.Get world
                let buttonName = menu.Name + "+" + itemCancelValue
                let button = menu.Parent / buttonName
                Content.button button.Name
                    [Entity.ParentNodeOpt == None
                     Entity.Visible <== menu.Visible
                     Entity.Size == v2 48.0f 48.0f
                     Entity.Position == Constants.Battle.CancelPosition
                     Entity.Elevation <== menu.Elevation
                     Entity.UpImage == asset Assets.Battle.PackageName (itemCancelValue + "Up")
                     Entity.DownImage == asset Assets.Battle.PackageName (itemCancelValue + "Down")
                     Entity.ClickEvent ==> cmd ItemCancel]]