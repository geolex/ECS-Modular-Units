using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

using Random = UnityEngine.Random;

public class FactoryManager : MonoBehaviour
{
    private EntityManager m_entityManager;


    // Archetypes
    private EntityArchetype m_partArchetype;
    //private EntityArchetype m_slotArchetype;

    [Header("Units to Spawn")]
    [SerializeField] List<Mesh> m_hulls;
    [SerializeField] List<Mesh> m_turrets;

    [SerializeField] string m_unitName = "";

    [Header("Materials")]
    [SerializeField] Material m_hullMaterial;
    [SerializeField] Material m_turretMaterial;

    // TEST DATA
    [SerializeField] private float3 m_turretSlotsTest;
    [SerializeField] private float3 m_weaponSlotsTest;
    //[SerializeField] private float3 m_trackSlotsTest;

    [SerializeField] private int m_nbTanks;







    // Start is called before the first frame update
    void Start()
    {
        int side = Mathf.CeilToInt(Mathf.Sqrt(m_nbTanks));


        m_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        m_partArchetype = m_entityManager.CreateArchetype(typeof(LocalToWorld), typeof(RenderMesh), typeof(Translation), typeof(Rotation), typeof(PartTypeComponent));

        //m_slotArchetype = m_entityManager.CreateArchetype(typeof(LocalToWorld), typeof(LocalToParent), typeof(Translation), typeof(Parent), typeof(PartTypeComponent));

        for(int i = 0; i < m_nbTanks; i++)
        {
            AssembleTank(new float3
            {
                x = (-3 * side) + (6* (i%side)),
                y = 0f, 
                z = 10 * Mathf.FloorToInt(i / side),
            });
                
        }


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AssembleTank()
    {
        AssembleTank(float3.zero);
    }

    private void AssembleTank(float3 _position)
    {
        // Create Hull
        Entity chimeraHull = m_entityManager.CreateEntity(m_partArchetype);
        //m_entityManager.AddComponent(chimeraHull, typeof(HubComponent));
        m_entityManager.AddComponent(chimeraHull, typeof(VelocityComponent));


        m_entityManager.SetSharedComponentData<RenderMesh>(chimeraHull, new RenderMesh { mesh = m_hulls[Random.Range(0, m_hulls.Count)], material = m_hullMaterial });
        //m_entityManager.SetComponentData<HubComponent>(chimeraHull, new HubComponent { m_turretSlots = m_turretSlotsTest});
        m_entityManager.SetComponentData<VelocityComponent>(chimeraHull, new VelocityComponent { m_value = new float3 { x = 0f, y = 0f, z = Random.Range(-2f, 5f) } });
        m_entityManager.SetComponentData<Translation>(chimeraHull, new Translation { Value = _position });
        m_entityManager.SetComponentData<Rotation>(chimeraHull, new Rotation { Value = quaternion.identity });


        OutfitTank(chimeraHull);
    }

    private void AssembleTank(float3 _position, string _name)
    {

    }

    private void OutfitTank(Entity _hull)
    {
        CreateTurrets(_hull);
    }


    
    private void CreateTurrets(Entity _hull)
    {
        Entity tempPart = m_entityManager.CreateEntity(m_partArchetype);

        // Point to the parent of the Object
        m_entityManager.AddComponent(tempPart, typeof(Parent));
        m_entityManager.SetComponentData<Parent>(tempPart, new Parent { Value = _hull });

        // Positions the turret on the tank
        m_entityManager.AddComponent(tempPart, typeof(LocalToParent));
        m_entityManager.SetComponentData<Translation>(tempPart, new Translation { Value = m_turretSlotsTest });

        // Handles the rotation characteristics of the turret
        m_entityManager.AddComponent(tempPart, typeof(TurretComponent));
	    m_entityManager.SetComponentData<TurretComponent>(tempPart, new TurretComponent { m_currentAngle = 0, m_maxAngularSpeed = 0.1f });

        // Handles the roations constraints of the slot
        m_entityManager.AddComponent(tempPart, typeof(SlotComponent));
        m_entityManager.SetComponentData<SlotComponent>(tempPart, new SlotComponent { m_maxAngleLeft = -3.14f, m_maxAngleRight = 3.14f });

       
        
        // Rendering
        m_entityManager.SetSharedComponentData<RenderMesh>(tempPart, new RenderMesh { mesh = m_turrets[Random.Range(0, m_turrets.Count)], material = m_turretMaterial });
    }




    #region JSON To Entities


    private void EntityFromJSon()
    {

    }



    #endregion






    #region LEGACY
    /*
    private void CreateWeapon(Entity _hull)
    {
        Entity tempPart = m_entityManager.CreateEntity(m_partArchetype);
        m_entityManager.AddComponent(tempPart, typeof(LocalToParent));
        m_entityManager.AddComponent(tempPart, typeof(Parent));

        m_entityManager.SetComponentData<Parent>(tempPart, new Parent { Value = _hull });
        m_entityManager.SetComponentData<Translation>(tempPart, new Translation { Value = m_weaponSlotsTest });

        m_entityManager.SetSharedComponentData<RenderMesh>(tempPart, new RenderMesh { mesh = m_weapons[Random.Range(0, m_weapons.Count)], material = m_weaponMaterial });
    }


    private void CreateTracks(Entity _hull, float3 _trackPosition)
    {
        Entity tempPart = m_entityManager.CreateEntity(m_partArchetype);

        m_entityManager.AddComponent(tempPart, typeof(LocalToParent));
        m_entityManager.AddComponent(tempPart, typeof(Parent));

        m_entityManager.SetComponentData<Parent>(tempPart, new Parent { Value = _hull });
        m_entityManager.SetComponentData<Translation>(tempPart, new Translation { Value = _trackPosition });

        m_entityManager.SetSharedComponentData<RenderMesh>(tempPart, new RenderMesh { mesh = m_tracks[Random.Range(0, m_tracks.Count)], material = m_trackMaterial });
    }
    */
    #endregion



}
