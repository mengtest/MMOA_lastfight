//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
//using System;
using BlGame.GameEntity;
using UnityEngine;
namespace BlGame.FSM
{
	public class EntityLocalMoveFSM:EntityFSM
	{
		public static readonly EntityLocalMoveFSM Instance = new EntityLocalMoveFSM();
		
		public FsmState State{
			get
			{
				return FsmState.FSM_STATE_LOCALMOVE;
			}
		}
		public bool CanNotStateChange{
			set;get;
		}
		public void Execute(Ientity ientity){
			//1.
			//entity.OnExecuteMove();
			//2.
//			Iselfplayer self = entity as Iselfplayer;
//			if (self != null) {
//				self.OnExecuteLocalMove ();
//			}

			Vector3 dir = ientity.TickMove ();

			ientity.TickRotate (dir);

		}

		public void Enter(Ientity entity , float stateLast){

		}
		public bool StateChange(Ientity entity , EntityFSM state){
			return CanNotStateChange;
		}

		public void Exit(Ientity ientity){
			ientity.TickExit ();
		}
	}
}

