/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

using nopact.Commons.Domain.Enum;
using nopact.Commons.Utility.Timer;
using UnityEngine;
using UnityEngine.EventSystems;

namespace nopact.Commons.UI
{
	public class VRInputModule : BaseInputModule {

		private PointerEventData lookData;
		private string selectionTimerId;
		private GameObject previousFrameSelectedHandler;

		public override void Process() {
		
			// send update events if there is a selected object - this is important for InputField to receive keyboard events
			SendUpdateEventToSelectedObject();
		
			ProcessLookEvent();
	
		}

		public override bool IsPointerOverGameObject(int pointerId) {

			return lookData != null && lookData.pointerEnter != null;

		}
	
		private bool SendUpdateEventToSelectedObject() {
		
			if (eventSystem.currentSelectedGameObject == null) {
			
				return false;

			}
			else {
			
				BaseEventData data = GetBaseEventData();
			
				ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
			
				return data.used;
			
			}
		
		}

		void ProcessSelect(GameObject currentRaycastTarget){

			HandlePointerExitAndEnter (lookData, currentRaycastTarget);

			var selectedHandler = ExecuteEvents.GetEventHandler<ISelectHandler> (currentRaycastTarget);

			if (selectedHandler != null) {

				VRSelectable vrSelectable = selectedHandler.GetComponent<VRSelectable> ();

				if (vrSelectable != null && vrSelectable.delayedSelection) {

					if (previousFrameSelectedHandler == null || previousFrameSelectedHandler != selectedHandler) {

						selectionTimerId = TimerUtility.Instance.RegisterTimer (vrSelectable.selectionDelay, CountdownScope.Menu,
							(s) => OnSelectTimerEnded (s));
					}

				}
				else {

					// Normal UI element
					// Select / submit as soon as pointer is on the UI element
					ExecuteEvents.ExecuteHierarchy (selectedHandler, lookData, ExecuteEvents.selectHandler);
					eventSystem.SetSelectedGameObject (selectedHandler);

				}
			}
			else {

				selectionTimerId = "";

			}

			previousFrameSelectedHandler = selectedHandler;

		}

		private void ProcessLookEvent() {

			lookData = GetLookPointerEventData();

			GameObject currentRaycastTarget = lookData.pointerCurrentRaycast.gameObject;
		
			ProcessSelect(currentRaycastTarget);

		}
	
		private PointerEventData GetLookPointerEventData() {
		
			Vector2 lookPosition;
			lookPosition.x = Screen.width/2;
			lookPosition.y = Screen.height/2;
		
			if (lookData == null) {
			
				lookData = new PointerEventData(eventSystem);
			
			}

			lookData.Reset();
			lookData.delta = Vector2.zero;
			lookData.position = lookPosition;
			lookData.scrollDelta = Vector2.zero;
		
			eventSystem.RaycastAll(lookData, m_RaycastResultCache);
			lookData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
			m_RaycastResultCache.Clear();
		
			return lookData;
		
		}

		private void OnSelectTimerEnded(string timerId) {

			if (selectionTimerId == timerId) {

				ExecuteEvents.ExecuteHierarchy(previousFrameSelectedHandler, lookData, ExecuteEvents.selectHandler);
				eventSystem.SetSelectedGameObject(previousFrameSelectedHandler);

			}

		}

	}
}
