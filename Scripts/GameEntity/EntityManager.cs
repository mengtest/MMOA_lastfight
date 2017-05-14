﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.17929
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
// </By JiangXiBolong>
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameDefine;
using JT.FWW.Tools;
using BlGame.GameData;
using JT.FWW.GameData;
using BlGame.GuideDate;
using BlGame.Ctrl;
using System;

namespace BlGame.GameEntity
{
    public class EntityManager
    {

        public static EntityManager Instance
        {
            private set;
            get;
        }
        public static Dictionary<UInt64, Ientity> AllEntitys = new Dictionary<UInt64, Ientity>();


        public enum CampTag
        {
            SelfCamp = 1,
            EnemyCamp = 0,
        }

        static int[] HOME_BASE_ID = { 21006, 21007, 21020, 21021 };

        private static List<Ientity> homeBaseList = new List<Ientity>();


        public EntityManager()
        {
            Instance = this;
        }

        public void ShowEntity(UInt64 sGUID, Vector3 pos, Vector3 dir)
        {
            if (!AllEntitys.ContainsKey(sGUID) || AllEntitys[sGUID].realObject == null)
            {
                return;
            }

            AllEntitys[sGUID].realObject.transform.position = pos;
            AllEntitys[sGUID].realObject.transform.rotation = Quaternion.LookRotation(dir);
            AllEntitys[sGUID].realObject.SetActive(true);

            if (AllEntitys[sGUID].FSM != null && AllEntitys[sGUID].FSM.State != BlGame.FSM.FsmState.FSM_STATE_DEAD)
            {
                AllEntitys[sGUID].ShowXueTiao();
            }
            else if (AllEntitys[sGUID].FSM != null && AllEntitys[sGUID].FSM.State == BlGame.FSM.FsmState.FSM_STATE_DEAD)
            {
                AllEntitys[sGUID].HideXueTiao();
            }

        }

        public void HideEntity(UInt64 sGUID)
        {
            if (!AllEntitys.ContainsKey(sGUID))
            {
                return;
            }

            Ientity entity = null;
            if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity) && entity.entityType == EntityType.Player)
            {
                if (PlayerManager.Instance.LocalAccount.ObType == ObPlayerOrPlayer.PlayerType)
                {
                    Iplayer.AddOrDelEnemy((Iplayer)entity,false);
                  //  UIEnemyTeamMateInfo.Instance.RemovePlayerEnemy((Iplayer)entity);
                }
            }

            //if (UIMiniMap.Instance != null)
            //{
            //    UIMiniMap.Instance.DestroyMapElement(sGUID);
            //}

            Iselfplayer player = PlayerManager.Instance.LocalPlayer;
            if (player != null && player.SyncLockTarget == AllEntitys[sGUID])
            {
                player.SetSyncLockTarget(null);
            }

            AllEntitys[sGUID].HideXueTiao();

            AllEntitys[sGUID].realObject.active = false;

        }

        public virtual Ientity HandleCreateEntity(UInt64 sGUID, EntityCampType campType)
        {//entity id
            return new Ientity(sGUID, campType);
        }

        public void DestoryAllEntity()
        {
            List<UInt64> keys = new List<UInt64>();
 
            foreach (Ientity entity in AllEntitys.Values)
            {
                if (entity.entityType != EntityType.Building)
                {
                    keys.Add(entity.GameObjGUID);
                }
            }

            foreach (UInt64 gui in keys)
            {
                HandleDelectEntity(gui);
            }
        }

        public static int HandleDelectEntity(UInt64 sGUID)
        {
            if (!AllEntitys.ContainsKey(sGUID))
            {
                return (int)ReturnRet.eT_DelEntityFailed;
            }
            Ientity entity = null;
            if (EntityManager.AllEntitys.TryGetValue(sGUID, out entity) && entity.entityType == EntityType.Player)
            {
                if (PlayerManager.Instance.LocalAccount.ObType == ObPlayerOrPlayer.PlayerType)
                {
                    Iplayer.AddOrDelEnemy((Iplayer)entity, false);
                }
            }
            Iselfplayer player = PlayerManager.Instance.LocalPlayer;
            if (player != null && player.SyncLockTarget == AllEntitys[sGUID])
            {
                player.SetSyncLockTarget(null);
            }
            if (entity.entityType == EntityType.Building)
            {
                MonoBehaviour.DestroyImmediate(AllEntitys[sGUID].realObject);
            }
            else {
                //删除GameObject 
                GameObjectPool.Instance.ReleaseGO(AllEntitys[sGUID].resPath, AllEntitys[sGUID].realObject, PoolObjectType.POT_Entity);
            }
            AllEntitys[sGUID].DestroyXueTiao();
            AllEntitys[sGUID] = null;
            AllEntitys.Remove(sGUID);

            return (int)ReturnRet.eT_Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public static void DelectBombBase(Ientity entity)
        {
            MonoBehaviour.DestroyImmediate(entity.realObject);
            entity.DestroyXueTiao();
            entity = null;
        }

        public void AddEntity(UInt64 sGUID, Ientity entity)
        {
            if (AllEntitys.ContainsKey(sGUID))
            {
                Debug.LogError("Has the same Guid: " + sGUID);
                return;
            }
            AllEntitys.Add(sGUID, entity);
        }

        public virtual Ientity GetEntity(UInt64 id)
        {
            Ientity entity;
            if (AllEntitys.TryGetValue(id, out entity))
            {
                return entity;
            }
            return null;
        }

        public GameObject CreateEntityModel(Ientity entity, UInt64 sGUID, Vector3 dir, Vector3 pos)
        {
            if (entity != null)
            {
                int id = (int)entity.ObjTypeID;
                this.SetCommonProperty(entity, id);
                if (entity.ModelName == null || entity.ModelName == "")
                {
                    return null;
                }
                string path = GameDefine.GameConstDefine.LoadMonsterModels;
                

                //创建GameObject    
                string resPath = path + entity.ModelName;
                entity.realObject = GameObjectPool.Instance.GetGO(resPath);
                if (entity.realObject == null)
                {
                    Debug.LogError("entity realObject is null");
                }

                //填充Entity信息
                entity.resPath = resPath;
                entity.objTransform = entity.realObject.transform;
                   
                entity.realObject.transform.localPosition = pos;
                entity.realObject.transform.localRotation = Quaternion.LookRotation(dir);
                                                           

                if (entity is Iplayer)
                {
                    entity.entityType = EntityType.Player;

                    if (entity is Iselfplayer)
                    {
                        PlayerManager.Instance.LocalPlayer = (Iselfplayer)entity;
                    }
                }
                else
                {
                    entity.entityType = (EntityType)ConfigReader.GetNpcInfo(id).NpcType;
                    if (entity.entityType == EntityType.Monster && (int)entity.EntityCamp >= (int)EntityCampType.CampTypeA)
                    {
                        entity.entityType = EntityType.AltarSoldier;
                    }
                    entity.ColliderRadius = ConfigReader.GetNpcInfo(id).NpcCollideRadius / 100f;
                }

                if (entity.NPCCateChild != ENPCCateChild.eNPCChild_BUILD_Shop)
                {
                    entity.CreateXueTiao();
                }
                
                AddEntityComponent(entity);
                return entity.realObject;
            }
            return null;
        }

        /// <summary>
        /// 设置entity基本属性、读取配置表
        /// </summary>
        /// <param name="id"></param>
        public virtual void SetCommonProperty(Ientity entity, int id)
        {
            entity.ModelName = GetModeName(id);
            entity.NpcGUIDType = id;
        }

        protected virtual string GetModeName(int id)
        {
            return null;
        }
		public string CacheGetModeName(int objTypeId){
			//String n = NpcManager.Instance.GetModeName (objTypeId);
			String n = null;
			if (string.IsNullOrEmpty(n)) {

				n = PlayerManager.Instance.GetModeName(objTypeId);
				return n;//it may be a modelname from cfg, but it also may be null value
			}else{
				return n;
			}
		}




        //建筑模型添加 entity
        public static Entity AddBuildEntityComponent(Ientity entity)
        {
            //添加Entity组件
            Entity syncEntity = (Entity)entity.realObject.AddComponent("Entity");         
            entity.RealEntity = syncEntity;
            syncEntity.SyncEntity = entity;
            syncEntity.CampType = entity.EntityCamp;
            syncEntity.sGUID = (int)entity.GameObjGUID;
            syncEntity.Type = entity.entityType;
            syncEntity.NPCCateChild = entity.NPCCateChild;
            return syncEntity;
        }


        public static void AddEntityComponent(Ientity entity)
        {
            //没有，添加Entity组件
            if (entity.realObject.GetComponent<Entity>() == null)
            {
                Entity syncEntity = entity.realObject.AddComponent<Entity>() as Entity;
                syncEntity.SyncEntity = entity;
                syncEntity.CampType = entity.EntityCamp;
                syncEntity.sGUID = (int)entity.GameObjGUID;
                syncEntity.Type = entity.entityType;
                syncEntity.NPCCateChild = entity.NPCCateChild;

                //添加动态模型优化组件 
                if (entity.entityType == EntityType.Monster || entity.entityType == EntityType.Soldier || entity.entityType == EntityType.AltarSoldier)
                {
                    OptimizeDynamicModel optimizeDynamicModel = entity.realObject.AddComponent<OptimizeDynamicModel>() as OptimizeDynamicModel;
                    syncEntity.dynModel = optimizeDynamicModel;
                }

                entity.RealEntity = syncEntity;
            }
            //直接取
            else
            {
                Entity syncEntity = entity.realObject.GetComponent<Entity>() as Entity;
                syncEntity.SyncEntity = entity;
                syncEntity.CampType = entity.EntityCamp;
                syncEntity.sGUID = (int)entity.GameObjGUID;
                syncEntity.Type = entity.entityType;
                syncEntity.NPCCateChild = entity.NPCCateChild;
                entity.RealEntity = syncEntity;
            }
        }



        public static void AddHomeBase(Ientity entity)
        {
            if (entity == null || entity.entityType != EntityType.Building)
                return;
            for (int i = 0; i < HOME_BASE_ID.Length; i++)
            {
                if (HOME_BASE_ID[i] == entity.NpcGUIDType)
                {
                    homeBaseList.Add(entity);
                    break;
                }
            }
        }

        public static void ClearHomeBase()
        {
            homeBaseList.Clear();
        }

        public static bool IsSelfHomeBase(Ientity entity)
        {
            if (entity == null)
                return false;
            if (PlayerManager.Instance.LocalPlayer != null && PlayerManager.Instance.LocalPlayer.EntityCamp != entity.EntityCamp)
                return false;
            for (int i = 0; i < HOME_BASE_ID.Length; i++)
            {
                if (HOME_BASE_ID[i] == entity.NpcGUIDType)
                {
                    return true;
                }
            }
            return false;
        }


        public static Ientity GetHomeBase(EntityCampType type)
        {
            foreach (var item in homeBaseList)
            {
                if (item.IsSameCamp(type))
                {
                    return item;
                }
            }
            return null;
        }

        public static List<Ientity> GetHomeBaseList()
        {
            return homeBaseList;
        }

    }
}