using System;
using System.IO;
using UnityEngine;

public class Controls : MonoBehaviour
{

	public float speed = 0f; //Скорость
	public float maxSpeed = 0.5f; //Максимальная скорость
	public float sideSpeed = 0f; //Боковая скорость

	public float scores = 0f; //Очки
	public float highScore = 0f; //Высший результат

	void Start()
	{
		string high = File.ReadAllText("highscore");

		try
		{
			highScore = Convert.ToSingle(high);
		}
		catch(Exception e) 
		{
			Debug.Log(e.ToString());
		}
		
	}

	void Update()
	{
		float moveSide = Input.GetAxis("Horizontal"); //Когда игрок будет нажимать на стрелочки влево или вправо, сюда будет добавляеться 1f или -1f
		float moveForward = Input.GetAxis("Vertical"); //То же само, но со стрелочками вверх и вниз

		if(moveSide != 0)
		{
			sideSpeed = moveSide * -1f; //Если игрок нажал на стрелочки влево или вправо, задаём боковую скорость
		}
		
		if(moveForward != 0)
		{
			speed += 0.01f * moveForward; //Если игрок нажал вверх или вниз
		}
		else //Если игрок не нажал ни вверх, ни вниз, то скорость будет постепенно возвращаться к нулю
		{
			if(speed > 0)
			{
				speed -= 0.01f;
			}
			else
			{
				speed += 0.01f;
			}
		}

		if(speed > maxSpeed)
		{
			speed = maxSpeed; //Проверка на превышение максимальной скорости
		}

	}
}
