using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Mov : MonoBehaviour
{

	public Rigidbody rb;
	public GameObject car; //Модель машины

	public GameObject brokenPrefab; //Префаб сломанной машины
	public GameObject modelHolder; //Объект, в который помещается модель

	public Controls control; //Скрипт управления

	private float speed = 0.1f; //Скорость на старте

	private float maxSpeed = 0.5f; //Максимальная скорость
	private float minSpeed = 0.1f; //Минимальная скорость

	private bool isAlive = true; //Жива ли машина. Если да, то она будет двигаться
	private bool isKilled = false; //Эта переменная нужна, чтобы триггер сработал только один раз

	public List<GameObject> wheels; //Колёса машины

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if (isAlive)
		{
			float newSpeed = speed; //Скорость движения вперёд
			float sideSpeed = 0f; //Скорость движения вбок

			if (control != null) //Если подключён скрипт управления
			{
				newSpeed += control.speed; //Изменение скорость
				sideSpeed = control.sideSpeed; //Изменение направления

				//car.GetComponent<AudioSource>().pitch = 2 + newSpeed; //Изменение высоты звука

				control.scores += 0.1f; //Добавление очков
			}

			if (newSpeed > maxSpeed)
			{
				newSpeed = maxSpeed; //Проверка на превышение максимальной скорости
			}

			if (newSpeed < minSpeed)
			{
				newSpeed = minSpeed; //Проверка на слишком низкую скорость
			}

			//Изменение положения машины - она двигается вперёд
			//Для этого к её положению по оси X прибавляется новая скорость, положение по Y остаётся прежним
			//К положение по оси Z прибавляется 0.1f, уможенная на боковую скорость 
			transform.position = new Vector3(transform.position.x + newSpeed, transform.position.y, transform.position.z + 0.1f * sideSpeed);

			if (control != null)
			{
				control.sideSpeed = 0f; //Сброс боковой скорости
			}

			if (wheels.Count > 0) //Если есть колёса
			{
				foreach (var wheel in wheels)
				{
					wheel.transform.Rotate(-3f, 0f, 0f); //Вращение каждого колеса по оси X
				}
			}

			if (tag == "Car")
			{
				if (transform.position.y < -50f || transform.position.x > 24.69 * 500f )//|| transform.position.y > 50f)
				{
					Destroy(gameObject); //Если это машина NPC, то она будет удаляться со сцены, если упадёт ниже -50f
				}
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		//Debug.Log("был вызван метод "+other.tag);
		if (other.tag == "Car" || other.tag == "Wall") //Если машина игрока столкнулась со стеной или другой машиной
		{
			isAlive = false; //Игрок больше не жив

			if (car != null) //Если есть модель
			{
				if (!isKilled) //Если триггер ещё не сработал
				{
					var pozicia = car.transform.position;
					var broken = Instantiate(brokenPrefab, pozicia, Quaternion.Euler(new Vector3(0f, -270f, 0f)));

					Destroy(car); //Удалить старую модель

					//Добавить новую модель

					
					broken.transform.SetParent(modelHolder.transform);

					isKilled = true; //Указать, что триггер сработал
					StartCoroutine("Die"); //Запустить процесс умирания
				}


			}
		}

		if (other.tag == "Coin") //Если столкновение с монетой
		{
			if (control != null) //Если столкнулась машина игрока
			{
				control.scores += 100f; //Добавить 100 очков

				other.GetComponent<Coin>().Delete(); //Удалить монету
			}
		}
	}

	IEnumerator Die() //Процесс умирания
	{
		string path = "highscore"; //Путь к файлу, в котором сохраняется высший результат
		using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
		{
			byte[] bytes = new byte[Convert.ToInt32(fs.Length)];

			fs.Read(bytes, 0, Convert.ToInt32(fs.Length));

			string high = Encoding.UTF8.GetString(bytes);

			float highScore = 0f;

			try
			{
				highScore = Convert.ToSingle(high);
			}
			catch (Exception e)
			{
				Debug.Log(e.ToString());
			}

			if (highScore < Math.Floor(control.scores))
			{
				byte[] newScores = Encoding.UTF8.GetBytes(Math.Floor(control.scores).ToString());

				fs.Write(newScores, 0, newScores.Length);
			}
		}

		yield return new WaitForSeconds(2f); //Подождать 2 секунды
		SceneManager.LoadScene("Menu"); //Перейти в меню
	}
}
