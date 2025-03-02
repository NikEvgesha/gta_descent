using UnityEngine;
using Newtonsoft.Json;
using Google.Apis.Json;

public class LinkerFix : MonoBehaviour
{
    void Start()
    {
        // Фиктивное использование типов для предотвращения стриппинга
        JsonConverter converter = new RFC3339DateTimeConverter();
        Debug.Log("Linker fix: " + converter.GetType());
    }
}