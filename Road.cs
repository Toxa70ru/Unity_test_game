using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Road : MonoBehaviour
{

	public List<GameObject> blocks; //Коллекция всех дорожных блоков
	public GameObject player; //Игрок
	public GameObject roadPrefab; //Префаб дорожного блока
	public GameObject carPrefab; //Префаб машины NPC
	public GameObject coinPrefab; //Префаб монеты

	private System.Random rand = new System.Random(); //Генератор случайных чисел
	

	void Update()
	{
		
		float x = player.GetComponent<Mov>().rb.position.x; //Получение положения игрока

		var last = blocks[blocks.Count - 1]; //Номер дорожного блока, который дальше всех от игрока

		if(x > last.transform.position.x - 24.69f * 10f  ) //Если игрок подъехал к последнему блоку ближе, чем на 10 блоков
		{
			if (x > blocks[0].transform.position.x + 24.69f * 1f)
			{
				blocks[0].GetComponent<RoadBlock>().Delete(); //Удаление блока со сцены
				blocks.Remove(blocks[0] as GameObject); //Удаление блока из коллекции
			}

			//Инстанцирование нового блока
			var block = Instantiate(roadPrefab, new Vector3(last.transform.position.x + 24.69f, last.transform.position.y, last.transform.position.z), Quaternion.identity); 
			block.transform.SetParent(gameObject.transform); //Перемещение блока в объект Road
			blocks.Add(block); //Добавление блока в коллекцию

			float side = rand.Next(1, 3) == 1 ? -1f : 1f; //Случаное определение стороны появления машины

			//Добавление машины на сцену
			var car = Instantiate(carPrefab, new Vector3(last.transform.position.x + 24.69f, last.transform.position.y + 0.20f, last.transform.position.z + 1.30f * side), Quaternion.Euler(new Vector3(0f, 90f, 0f)));
			car.transform.SetParent(gameObject.transform); //Добавление машины в объект Road

			if(rand.Next(0, 100) > 70) //Добавление монеты с вероятностью 30%
			{
				var coin = Instantiate(coinPrefab, new Vector3(last.transform.position.x + 24.69f, last.transform.position.y + 0.20f, last.transform.position.z + 1.50f * side * -1f), Quaternion.identity);
				coin.transform.SetParent(gameObject.transform);
			}

		}

	}
}
