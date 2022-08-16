using ThunderRoad;
using UnityEngine;

namespace HFBlade
{
    public class MurasamaModule : ItemModule
    {
        public float Speed;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<MurasamaComponent>().Speed = Speed;
        }
    }
    public class ImbueModule : ItemModule
    {
        public string SpellID = null;
        public bool onStart = false;
        public bool onUnsheathe = false;
        public bool onUpdate = false;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ImbueComponent>().Setup(SpellID, onStart, onUnsheathe, onUpdate);
        }
    }
    public class ImbueComponent : MonoBehaviour
    {
        string spellId;
        bool onStart;
        bool onUnsheath;
        bool onUpdate;
        Item item;
        SpellCastCharge spell;
        public void Start()
        {
            item = GetComponent<Item>();
            spell = Catalog.GetData<SpellCastCharge>(spellId);
            if(onStart)
            {
                item.colliderGroups[0].imbue.Transfer(spell, item.colliderGroups[0].imbue.maxEnergy);
            }
            item.OnUnSnapEvent += Item_OnUnSnapEvent;
        }

        private void Item_OnUnSnapEvent(Holder holder)
        {
            if(onUnsheath)
            item.colliderGroups[0].imbue.Transfer(spell, item.colliderGroups[0].imbue.maxEnergy);
        }
        public void Update()
        {
            if (onUpdate)
            {
                item.colliderGroups[0].imbue.Transfer(spell, item.colliderGroups[0].imbue.maxEnergy);
            }
        }
        public void Setup(string spellID, bool start, bool unsheath, bool update)
        {
            spellId = spellID;
            onStart = start;
            onUnsheath = unsheath;
            onUpdate = update;
        }
    }
    public class MurasamaComponent : MonoBehaviour
    {
        public float Speed;
        Item item;
        Holder holder;
        public void Start()
        {
            item = GetComponent<Item>();
            holder = item.GetComponentInChildren<Holder>();
            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }
        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if(action == Interactable.Action.UseStart && holder.items.Count == 1 && holder.items[0] is Item sword)
            {
                sword.IgnoreObjectCollision(item);
                sword.IgnoreRagdollCollision(ragdollHand.ragdoll);
                holder.UnSnap(sword);
                sword.rb.AddForce(-sword.holderPoint.forward * Speed, ForceMode.Impulse);
                sword.isThrowed = true;
                sword.SetColliderAndMeshLayer(GameManager.GetLayer(LayerName.MovingItem));
                sword.rb.collisionDetectionMode = Catalog.gameData.collisionDetection.throwed;
                if (!Item.allThrowed.Contains(sword)) Item.allThrowed.Add(sword);
            }
        }
    }
}
