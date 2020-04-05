function GetFirstEquippableItem(actor)
	for i=0,actor.Items.Count -1 do
		if actor:CanEquip(actor.Items[i]) then
			return actor.Items[i]
		end
	end

	return nil
end

function FirstOrDefault(list)
	if list.Count > 0 then
		return list[0]
	end

	return nil
end

function SplashDamage(injurySystem,amount,affectsRelationships)

	injurySystem:ApplyToAll(Room.Actors,SystemArgs(World,UserInterface,20,AggressorIfAny,null,Round))

    if affectsRelationships then
		World.Relationships:ApplyToAll(Room.Actors,SystemArgs(World,UserInterface,-10,AggressorIfAny,null,Round))
	end
end