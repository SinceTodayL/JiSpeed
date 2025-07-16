namespace JISpeed.Core.Constants
{
	// �����볣������
	public static class ErrorCodes
	{
		/*1xxx: ͨ������/��������*/
		public const int GeneralRequestError = 1000;      //ͨ���������
		public const int ParameterValidationFailed = 1001;//������֤ʧ��
		public const int MissingRequiredParameter = 1002; //ȱ�ٱ������
		public const int InvalidRequestFormat = 1003;     //��Ч�������ʽ
		public const int MethodNotSupported = 1004;       //��֧�ֵ�HTTP����

		/*2xxx: ��֤����Ȩ����*/
		public const int GeneralAuthError = 2000;         //ͨ����֤����
		public const int Unauthorized = 2001;             //δ��Ȩ����
		public const int Forbidden = 2002;                //��ֹ���ʣ���Ȩ�ޣ�
		public const int AccountDisabled = 2003;          //�˺��ѽ���
		public const int InvalidCredentials = 2004;       //��Ч��ƾ�ݣ��û���/�������

		/*3xxx: ҵ���߼�����*/
		public const int GeneralBusinessError = 3000;     //ͨ��ҵ���߼�����
		public const int ResourceAlreadyExists = 3001;    //��Դ�Ѵ���
		public const int ResourceNotFound = 3002;         //��Դδ�ҵ�
		public const int InsufficientStock = 3003;        //��治��
		public const int InvalidState = 3004;             //��Ч��״̬
		public const int InsufficientBalance = 3005;      //����
		public const int TooManyRequests = 3006;          //�������Ƶ��

		/*5xxx: ϵͳ/�����ڲ�����*/
		public const int GeneralSystemError = 5000;       //ͨ��ϵͳ�ڲ�����
		public const int DatabaseError = 5001;            //���ݿ��������
		public const int ThirdPartyServiceError = 5002;   //�������������
		public const int ServiceUnavailable = 5003;       //���񲻿���
	}
}
