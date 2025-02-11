﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;

namespace HealthV2
{
	[CreateAssetMenu(fileName = "ImplantProcedure", menuName = "ScriptableObjects/Surgery/ImplantProcedure")]
	public class ImplantProcedure : SurgeryProcedureBase
	{

		public ItemTrait RequiredImplantTrait;


		public override void FinnishSurgeryProcedure(BodyPart OnBodyPart, HandApply interaction,
			Dissectible.PresentProcedure PresentProcedure)
		{
			base.FinnishSurgeryProcedure(OnBodyPart, interaction, PresentProcedure);

			var itemApp = interaction?.HandSlot?.Item.OrNull()?.GetComponent<ItemAttributesV2>();

			ItemSlot ToTakeFrom = null;
			if (interaction?.HandSlot?.Item != null )
			{
				if (itemApp.OrNull()?.HasTrait(RequiredImplantTrait) == true)
				{
					ToTakeFrom = interaction.HandSlot;
				}

				if (itemApp.OrNull()?.HasTrait(CommonTraits.Instance.ItemBag) == true)
				{
					var Slot = interaction?.HandSlot?.Item.GetComponent<ItemStorage>().GetItemSlots().First(); //It has baggy it should have item storage
					if (Slot.Item != null)
					{
						itemApp = Slot.Item.OrNull()?.GetComponent<ItemAttributesV2>();
						if (itemApp.OrNull()?.HasTrait(RequiredImplantTrait) == true)
						{
							ToTakeFrom = Slot;
						}
					}
				}
			}

			if (ToTakeFrom != null)
			{
				if (OnBodyPart != null)
				{
					OnBodyPart.OrganStorage.ServerTryTransferFrom(ToTakeFrom);
				}
				else
				{
					var health = PresentProcedure.isOn.GetComponent<LivingHealthMasterBase>();

					if (itemApp.HasTrait(CommonTraits.Instance.CoreBodyPart))
					{
						if (health.HasCoreBodyPart()) return;
						health.BodyPartStorage.ServerTryTransferFrom(ToTakeFrom);
						PresentProcedure.isOn.currentlyOn = null;
					}
					else
					{
						health.BodyPartStorage.ServerTryTransferFrom(ToTakeFrom);
						PresentProcedure.isOn.currentlyOn = null;
					}
				}
			}
		}
	}
}