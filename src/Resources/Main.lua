function GetFirstEquippableItem(actor)
	for i=0,actor.Items.Count -1 do
		if actor:CanEquip(actor.Items[i]) then
			return actor.Items[i]
		end
	end
end