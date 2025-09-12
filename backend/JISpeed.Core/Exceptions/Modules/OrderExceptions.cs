using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // ����ģ���쳣��
    public class OrderExceptions
    {
        // ��������δ�ҵ��쳣
        // orderId: ����ID
        // ����: δ�ҵ��쳣
        public static NotFoundException OrderNotFound(string orderId)
        {
            return new NotFoundException(ErrorCodes.OrderNotFound, $"���� (ID: {orderId}) δ�ҵ�");
        }

        // ��������״̬�����쳣
        // orderId: ����ID
        // currentStatus: ��ǰ״̬
        // requiredStatus: ����״̬
        // ����: ҵ���쳣
        public static BusinessException OrderStatusError(string orderId, int currentStatus, int requiredStatus)
        {
            string statusText = currentStatus == 0 ? "δ֧��" :
                               (currentStatus == 1 ? "��֧��" :
                               (currentStatus == 2 ? "��ȷ��" :
                               (currentStatus == 3 ? "������" :
                               (currentStatus == 4 ? "�ۺ���" :
                               (currentStatus == 5 ? "�ۺ����" :
                               (currentStatus == 6 ? "��ȡ��" :
                               (currentStatus == 7 ? "���ɵ�" :
                               (currentStatus == 8 ? "������" :
                               (currentStatus == 9 ? "�������ʹ�" : "δ֪״̬")))))))));

            string requiredText = requiredStatus == 0 ? "δ֧��" :
                                 (requiredStatus == 1 ? "��֧��" :
                                 (requiredStatus == 2 ? "��ȷ��" :
                                 (requiredStatus == 3 ? "������" :
                                 (requiredStatus == 4 ? "�ۺ���" :
                                 (requiredStatus == 5 ? "�ۺ����" :
                                 (requiredStatus == 6 ? "��ȡ��" :
                                 (requiredStatus == 7 ? "���ɵ�" :
                                 (requiredStatus == 8 ? "������" :
                                 (requiredStatus == 9 ? "�������ʹ�" : "δ֪״̬")))))))));

            return new BusinessException(ErrorCodes.OrderStatusError,
                $"���� (ID: {orderId}) ��ǰ״̬ ({statusText}) ������˲�������Ҫ״̬: {requiredText}");
        }

        // ����������ȡ���쳣
        // orderId: ����ID
        // ����: ҵ���쳣
        public static BusinessException OrderCancelled(string orderId)
        {
            return new BusinessException(ErrorCodes.OrderCancelled,
                $"���� (ID: {orderId}) �ѱ�ȡ�����޷�ִ�д˲���");
        }

        // ���������ѷ����쳣
        // orderId: ����ID
        // riderId: ����ID
        // ����: ҵ���쳣
        public static BusinessException OrderAlreadyAssigned(string orderId, string riderId)
        {
            return new BusinessException(ErrorCodes.OrderAlreadyAssigned,
                $"���� (ID: {orderId}) �ѷ�������� (ID: {riderId})");
        }

        // ������������ʧ���쳣
        // orderId: ����ID
        // riderId: ����ID
        // reason: ʧ��ԭ��
        // ����: ҵ���쳣
        public static BusinessException OrderAssignmentFailed(string orderId, string riderId, string reason)
        {
            return new BusinessException(ErrorCodes.OrderAssignmentFailed,
                $"���� (ID: {orderId}) ��������� (ID: {riderId}) ʧ��: {reason}");
        }

        // ���������������쳣
        // orderId: ����ID
        // expectedAmount: Ԥ�ڽ��
        // actualAmount: ʵ�ʽ��
        // ����: ҵ���쳣
        public static BusinessException OrderAmountError(string orderId, decimal expectedAmount, decimal actualAmount)
        {
            return new BusinessException(ErrorCodes.OrderAmountError,
                $"���� (ID: {orderId}) ������Ԥ��: {expectedAmount}��ʵ��: {actualAmount}");
        }

        // ����������ʱ�쳣
        // orderId: ����ID
        // ����: ҵ���쳣
        public static BusinessException OrderTimeout(string orderId)
        {
            return new BusinessException(ErrorCodes.OrderTimeout,
                $"���� (ID: {orderId}) ����ʱ");
        }

        // ����������Ʒ�����쳣
        // orderId: ����ID
        // dishId: ��ƷID
        // reason: ����ԭ��
        // ����: ҵ���쳣
        public static BusinessException OrderItemError(string orderId, string dishId, string reason)
        {
            return new BusinessException(ErrorCodes.OrderItemError,
                $"���� (ID: {orderId}) ��Ʒ (ID: {dishId}) ����: {reason}");
        }

        // ����������ַ��Ч�쳣
        // orderId: ����ID
        // addressId: ��ַID
        // ����: ҵ���쳣
        public static BusinessException OrderAddressInvalid(string orderId, string addressId)
        {
            return new BusinessException(ErrorCodes.OrderAddressInvalid,
                $"���� (ID: {orderId}) ��ַ (ID: {addressId}) ��Ч");
        }

        // ������������ʧ���쳣
        // userId: �û�ID
        // reason: ʧ��ԭ��
        // ����: ҵ���쳣
        public static BusinessException OrderCreationFailed(string userId, string reason)
        {
            return new BusinessException(ErrorCodes.OrderCreationFailed,
                $"�û� (ID: {userId}) ��������ʧ��: {reason}");
        }
    }
}
