using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // �û�ģ���쳣��
    public class UserExceptions
    {
        // �����û�δ�ҵ��쳣
        // userId: �û�ID
        // ����: δ�ҵ��쳣
        public static NotFoundException UserNotFound(string userId)
        {
            return new NotFoundException(ErrorCodes.UserNotFound, $"�û� (ID: {userId}) δ�ҵ�");
        }

        // �����û�δ�ҵ��쳣(ͨ���˺�)
        // account: �û��˺�
        // ����: δ�ҵ��쳣
        public static NotFoundException UserNotFoundByAccount(string account)
        {
            return new NotFoundException(ErrorCodes.UserNotFound, $"�û� (�˺�: {account}) δ�ҵ�");
        }

        // �����û�״̬�����쳣
        // userId: �û�ID
        // status: ��ǰ״̬
        // ����: ҵ���쳣
        public static BusinessException UserStatusError(string userId, int status)
        {
            string statusText = status == 1 ? "��Ծ" : status == 2 ? "��ɾ��" : "δ֪";
            return new BusinessException(ErrorCodes.UserStatusError,
                $"�û� (ID: {userId}) ��ǰ״̬ ({statusText}) ������ִ�д˲���");
        }

        // �����û���ַδ�ҵ��쳣
        // addressId: ��ַID
        // userId: �û�ID
        // ����: δ�ҵ��쳣
        public static NotFoundException UserAddressNotFound(string addressId, string? userId = null)
        {
            if (userId != null)
                return new NotFoundException(ErrorCodes.UserAddressNotFound, $"�û� (ID: {userId}) �ĵ�ַ (ID: {addressId}) δ�ҵ�");
            else
                return new NotFoundException(ErrorCodes.UserAddressNotFound, $"��ַ (ID: {addressId}) δ�ҵ�");
        }

        // �����û������쳣
        // userId: �û�ID
        // requiredAmount: ������
        // currentBalance: ��ǰ���
        // ����: ҵ���쳣
        public static BusinessException UserInsufficientBalance(string userId, decimal requiredAmount, decimal currentBalance)
        {
            return new BusinessException(ErrorCodes.UserInsufficientBalance,
                $"�û� (ID: {userId}) ���㣬��Ҫ: {requiredAmount}����ǰ���: {currentBalance}");
        }

        // �����û���Ϣ�������쳣
        // userId: �û�ID
        // missingFields: ȱʧ���ֶ�
        // ����: ҵ���쳣
        public static BusinessException UserIncompleteInfo(string userId, string missingFields)
        {
            return new BusinessException(ErrorCodes.UserIncompleteInfo,
                $"�û� (ID: {userId}) ��Ϣ��������ȱ��: {missingFields}");
        }

        // �����û��Ѵ����쳣
        // account: �û��˺�
        // ����: ҵ���쳣
        public static BusinessException UserAlreadyExists(string account)
        {
            return new BusinessException(ErrorCodes.UserAlreadyExists,
                $"�û��˺� '{account}' �Ѵ���");
        }

        // �����û���֤������쳣
        // account: �û��˺�
        // ����: ҵ���쳣
        public static BusinessException UserVerificationCodeError(string account)
        {
            return new BusinessException(ErrorCodes.UserVerificationCodeError,
                $"�û� (�˺�: {account}) �ṩ����֤�벻��ȷ");
        }

        // �����û���¼ʧ���쳣
        // account: �û��˺�
        // reason: ʧ��ԭ��
        // ����: ҵ���쳣
        public static BusinessException UserLoginFailed(string account, string? reason = null)
        {
            if (string.IsNullOrEmpty(reason))
                return new BusinessException(ErrorCodes.UserLoginFailed,
                    $"�û� (�˺�: {account}) ��¼ʧ�ܣ��û������������");
            else
                return new BusinessException(ErrorCodes.UserLoginFailed,
                    $"�û� (�˺�: {account}) ��¼ʧ�ܣ�ԭ��: {reason}");
        }
    }
}
