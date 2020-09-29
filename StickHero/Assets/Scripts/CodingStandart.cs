using UnityEngine;
using System.Collections;
using System;

//Здесь описываем кодинг стандарт
public class CodingStandart : MonoBehaviour 
{//фигурные скобки начинаем с новой строки
	[SerializeField] private float m_FloatVariable; 	//поля для редактора начинаем с m_
	public float FloatVariable                          //проперти пишем с большой буквы
    {					
		get{return m_FloatVariable;}
		set{m_FloatVariable = value;}
	}

	private float myPrivateFloat; 				//локальные переменные начинаем с маленькой буквы

	private Action someAction;

	public Action<float> FloatAction;


	private void Reset()
	{
		m_FloatVariable = 1f; 					//здесь указываем значения по умолчанию для переменных - вызывается при добавлении на объект
	}											//можно вызвать из контексного меню редактора что бы сбросить поля
	private void OnEnable()
	{
		someAction += DoSomthingHere;
		FloatAction += FloatActionMethod;
	}

	private void OnDisable()
	{
		someAction -= DoSomthingHere;
		FloatAction -= FloatActionMethod;
	} 

	// Use this for initialization
	private void Start () 
	{
		someAction.Invoke();
		FloatAction.Invoke(5f);
	}
	
	// Update is called once per frame
	private void Update () 
	{
		//если апдейт и старт не используются - их необходимо удалить
	}

	//этот паблик метод ничего не делает
	public void DoSomthing()
	{

	}

	//этот ллокальный метод ничего не делает
	private void DoSomthingHere()
	{

	}

	private void FloatActionMethod(float value)
	{

	}
}
