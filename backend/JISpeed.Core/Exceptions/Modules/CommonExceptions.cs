using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // ͨ�ô����쳣��
    public class CommonExceptions
    {
        // ����ͨ�ô����쳣
        // message: ������Ϣ
        // ����: ҵ���쳣
        public static BusinessException GeneralError(string message)
        {
            return new BusinessException(ErrorCodes.GeneralError, message);
        }

        // ����������֤ʧ���쳣
        // fieldName: �ֶ�����
        // reason: ��֤ʧ��ԭ��
        // ����: ��֤�쳣
        public static ValidationException ValidationFailed(string fieldName, string reason)
        {
            return new ValidationException(ErrorCodes.ValidationFailed, $"�ֶ� '{fieldName}' ��֤ʧ��: {reason}");
        }

        // ����ȱ�ٱ�������쳣
        // paramName: ��������
        // ����: ��֤�쳣
        public static ValidationException MissingParameter(string paramName)
        {
            return new ValidationException(ErrorCodes.MissingParameter, $"ȱ�ٱ������: {paramName}");
        }

        // ������Ч�����ʽ�쳣
        // message: ������Ϣ
        // ����: ҵ���쳣
        public static BusinessException InvalidRequestFormat(string message)
        {
            return new BusinessException(ErrorCodes.InvalidRequestFormat, $"��Ч�������ʽ: {message}");
        }

        // ������֧�ֵĲ����쳣
        // operation: ��������
        // ����: ҵ���쳣
        public static BusinessException UnsupportedOperation(string operation)
        {
            return new BusinessException(ErrorCodes.UnsupportedOperation, $"��֧�ֵĲ���: {operation}");
        }

        // ��������Ƶ�������쳣
        // ipAddress: IP��ַ
        // maxRequests: ����������
        // timeWindow: ʱ�䴰��(��)
        // ����: ҵ���쳣
        public static BusinessException RateLimitExceeded(string ipAddress, int maxRequests, int timeWindow)
        {
            return new BusinessException(ErrorCodes.RateLimitExceeded,
                $"�������Ƶ����IP: {ipAddress}����������: {maxRequests}��/{timeWindow}�룬���Ժ�����");
        }
    }
}
