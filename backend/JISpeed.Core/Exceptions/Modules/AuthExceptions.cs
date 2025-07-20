using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
	// ��֤����Ȩ�쳣��
	public class AuthExceptions
	{
		// ����δ��Ȩ�����쳣
		// userId: �û�ID
		// ����: ҵ���쳣
		public static BusinessException Unauthorized(string userId = null)
		{
			return new BusinessException(ErrorCodes.Unauthorized,
				userId == null ? "δ��Ȩ���ʣ����ȵ�¼" : $"�û� (ID: {userId}) δ��Ȩ���ʣ����ȵ�¼");
		}

		// ������ֹ�����쳣
		// userId: �û�ID
		// resource: ��Դ����
		// ����: ҵ���쳣
		public static BusinessException Forbidden(string userId, string resource)
		{
			return new BusinessException(ErrorCodes.Forbidden,
				$"�û� (ID: {userId}) ��Ȩ������Դ: {resource}");
		}

		// �����˺��ѽ����쳣
		// userId: �û�ID
		// account: �˺�
		// ����: ҵ���쳣
		public static BusinessException AccountDisabled(string userId, string account)
		{
			return new BusinessException(ErrorCodes.AccountDisabled,
				$"�˺� (ID: {userId}, �˺�: {account}) �ѱ����ã�����ϵ����Ա");
		}

		// ������Чƾ���쳣
		// account: �˺�
		// ����: ҵ���쳣
		public static BusinessException InvalidCredentials(string account)
		{
			return new BusinessException(ErrorCodes.InvalidCredentials,
				$"�˺� ({account}) ���������");
		}

		// ���������ѹ����쳣
		// userId: �û�ID
		// ����: ҵ���쳣
		public static BusinessException TokenExpired(string userId)
		{
			return new BusinessException(ErrorCodes.TokenExpired,
				$"�û� (ID: {userId}) ����֤�����ѹ��ڣ������µ�¼");
		}

		// ������Ч�����쳣
		// reason: ��Чԭ��
		// ����: ҵ���쳣
		public static BusinessException InvalidToken(string reason)
		{
			return new BusinessException(ErrorCodes.InvalidToken,
				$"��Ч����֤����: {reason}");
		}
	}
}
