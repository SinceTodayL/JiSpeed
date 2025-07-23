using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // ϵͳ�����쳣��
    public class SystemExceptions
    {
        // ����ϵͳ�ڲ������쳣
        // message: ������Ϣ
        // ����: ҵ���쳣
        public static BusinessException SystemError(string message)
        {
            return new BusinessException(ErrorCodes.SystemError, $"ϵͳ�ڲ�����: {message}");
        }

        // �������ݿ�����쳣
        // tableName: ����
        // operation: ��������
        // message: ������Ϣ
        // ����: ҵ���쳣
        public static BusinessException DatabaseError(string tableName, string operation, string message)
        {
            return new BusinessException(ErrorCodes.DatabaseError,
                $"���ݿ����: �� '{tableName}' {operation}����ʧ�� - {message}");
        }

        // ������������쳣
        // key: �����
        // operation: ��������
        // ����: ҵ���쳣
        public static BusinessException CacheError(string key, string operation)
        {
            return new BusinessException(ErrorCodes.CacheError,
                $"����������: �� '{key}' {operation}����ʧ��");
        }

        // ������������������쳣
        // serviceName: ��������
        // operation: ��������
        // message: ������Ϣ
        // ����: ҵ���쳣
        public static BusinessException ExternalServiceError(string serviceName, string operation, string message)
        {
            return new BusinessException(ErrorCodes.ExternalServiceError,
                $"�������������: {serviceName} {operation}����ʧ�� - {message}");
        }

        // ������������쳣
        // endpoint: Ŀ��˵�
        // message: ������Ϣ
        // ����: ҵ���쳣
        public static BusinessException NetworkError(string endpoint, string message)
        {
            return new BusinessException(ErrorCodes.NetworkError,
                $"����ͨ�Ŵ���: ���� {endpoint} ʧ�� - {message}");
        }

        // �������ô����쳣
        // configKey: ���ü�
        // message: ������Ϣ
        // ����: ҵ���쳣
        public static BusinessException ConfigurationError(string configKey, string message)
        {
            return new BusinessException(ErrorCodes.ConfigurationError,
                $"���ô���: �� '{configKey}' - {message}");
        }

        // �������񲻿����쳣
        // serviceName: ��������
        // ����: ҵ���쳣
        public static BusinessException ServiceUnavailable(string serviceName)
        {
            return new BusinessException(ErrorCodes.ServiceUnavailable,
                $"���񲻿���: {serviceName} ��ǰ�޷����ʣ����Ժ�����");
        }

        // �����ļ����������쳣
        // filePath: �ļ�·��
        // operation: ��������
        // message: ������Ϣ
        // ����: ҵ���쳣
        public static BusinessException FileOperationError(string filePath, string operation, string message)
        {
            return new BusinessException(ErrorCodes.FileOperationError,
                $"�ļ���������: �ļ� '{filePath}' {operation}ʧ�� - {message}");
        }

        // ������Ϣ���д����쳣
        // queueName: ��������
        // operation: ��������
        // message: ������Ϣ
        // ����: ҵ���쳣
        public static BusinessException MessageQueueError(string queueName, string operation, string message)
        {
            return new BusinessException(ErrorCodes.MessageQueueError,
                $"��Ϣ���д���: ���� '{queueName}' {operation}ʧ�� - {message}");
        }

        // ������ͼ��������쳣
        // operation: ��������
        // message: ������Ϣ
        // ����: ҵ���쳣
        public static BusinessException MapServiceError(string operation, string message)
        {
            return new BusinessException(ErrorCodes.MapServiceError,
                $"��ͼ�������: {operation}ʧ�� - {message}");
        }
    }
}
