﻿/*
 * Created on Mon Jan 27 2020
 *
 * The MIT License (MIT)
 * Copyright (c) 2020 Sahil Jain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using GameplayAbilitySystem.Abilities.ScriptableObjects;
using GameplayAbilitySystem.Common.Editor;
using GameplayAbilitySystem.GameplayTags.Components;
using GameplayAbilitySystem.GameplayTags.Interfaces;
using GameplayAbilitySystem.GameplayTags.ScriptableObjects;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

public class AbilitiesToEntitiesAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity {

    [SerializeField]
    private List<AbilityScriptableObject> Abilities;

    private Dictionary<Type, Entity> Entities;

    public Entity EntityForType(Type t) {
        return Entities[t];
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
        var archetype = dstManager.CreateArchetype(typeof(GameplayTagsBufferElement<IAbilityTagsBufferElement>));
        for (int i = 0; i < Abilities.Count; i++) {
            // Create entity for each ability type
            var abilitySO = Abilities[i];
            var abilityEntity = dstManager.CreateEntity(typeof(GameplayTagsBufferElement<IAbilityTagsBufferElement>));
            dstManager.AddBuffer<GameplayTagsBufferElement<IAbilityTagsBufferElement>>(abilityEntity);
            var gameplayTagArray = new NativeArray<GameplayTagsBufferElement<IAbilityTagsBufferElement>>(abilitySO.AbilityTags.Count, Allocator.Temp);
            var dBuffer = dstManager.GetBuffer<GameplayTagsBufferElement<IAbilityTagsBufferElement>>(abilityEntity);
            for (var j = 0; j < abilitySO.AbilityTags.Count; j++) {
                var tag = abilitySO.AbilityTags[j].GameplayTag;
                var tagComponent = new GameplayTagsBufferElement<IAbilityTagsBufferElement>() { Value = tag.GameplayTagComponent, SourceGameplayEffectEntity = abilityEntity };
                dBuffer.Add(tagComponent);
            }
            dstManager.SetName(abilityEntity, GetDisplayNameForType(abilitySO.AbilityType.ComponentType.GetManagedType()));
        }
    }

    private string GetDisplayNameForType(Type type) {
        var displayNameAttribute = (AbilitySystemDisplayNameAttribute)System.Attribute.GetCustomAttribute(type, typeof(AbilitySystemDisplayNameAttribute));
        string displayName = "";
        if (displayNameAttribute != null) {
            displayName = displayNameAttribute.Name;
        }

        return displayName;
    }

}

// [CustomEditor(typeof(AbilitiesToEntitiesAuthoringComponent))]
// public class AbilitiesSerializerEditor : Editor {

//     public override void OnInspectorGUI() {
//         DrawDefaultInspector();
//         var t = (AbilitiesToEntitiesAuthoringComponent)target;
//         if (GUILayout.Button("Update All Abilities")) {
//             t.LoadAbilitiesAndUpdate();
//         }
//     }


// public class ComponentCollector {
//     public IEnumerable<System.Type> GetAllTypes(System.AppDomain domain) {
//         var componentInterface = typeof(IAbilityTagComponent);
//         var types = domain.GetAssemblies()
//                     .SelectMany(s => s.GetTypes())
//                     .Where(p => componentInterface.IsAssignableFrom(p) && !p.IsInterface);
//         return types;
//     }
// }
// }