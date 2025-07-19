using JISpeed.Core.Constants;
using System;

namespace JISpeed.Core.Exceptions
{
    // ֧��ģ���쳣��
    public class PaymentExceptions
    {
        // ����֧��ʧ���쳣
        // payId: ֧��ID
        // reason: ʧ��ԭ��
        // ����: ҵ���쳣
        public static BusinessException PaymentFailed(string payId, string reason)
        {
            return new BusinessException(ErrorCodes.PaymentFailed,
                $"֧�� (ID: {payId}) ����ʧ��: {reason}");
        }

        // ����֧����ʱ�쳣
        // payId: ֧��ID
        // ����: ҵ���쳣
        public static BusinessException PaymentTimeout(string payId)
        {
            return new BusinessException(ErrorCodes.PaymentTimeout,
                $"֧�� (ID: {payId}) ����ʱ�����Ժ��ѯ֧�����");
        }

        // �����˿�ʧ���쳣
        // refundId: �˿�ID
        // reason: ʧ��ԭ��
        // ����: ҵ���쳣
        public static BusinessException RefundFailed(string refundId, string reason)
        {
            return new BusinessException(ErrorCodes.RefundFailed,
                $"�˿� (ID: {refundId}) ����ʧ��: {reason}");
        }

        // ����֧����ʽ��֧���쳣
        // channel: ֧������
        // ����: ҵ���쳣
        public static BusinessException PaymentMethodNotSupported(string channel)
        {
            return new BusinessException(ErrorCodes.PaymentMethodNotSupported,
                $"��֧�ֵ�֧����ʽ: {channel}");
        }

        // �������׼�¼δ�ҵ��쳣
        // payId: ֧��ID
        // ����: δ�ҵ��쳣
        public static NotFoundException TransactionNotFound(string payId)
        {
            return new NotFoundException(ErrorCodes.TransactionNotFound,
                $"���׼�¼ (ID: {payId}) δ�ҵ�");
        }

        // �����ظ�֧���쳣
        // orderId: ����ID
        // payId: ��֧����֧��ID
        // ����: ҵ���쳣
        public static BusinessException DuplicatePayment(string orderId, string payId)
        {
            return new BusinessException(ErrorCodes.DuplicatePayment,
                $"���� (ID: {orderId}) ��֧�� (֧��ID: {payId})�������ظ�֧��");
        }

        // ����֧���������쳣
        // payId: ֧��ID
        // expectedAmount: Ԥ�ڽ��
        // actualAmount: ʵ�ʽ��
        // ����: ҵ���쳣
        public static BusinessException PaymentAmountError(string payId, decimal expectedAmount, decimal actualAmount)
        {
            return new BusinessException(ErrorCodes.PaymentAmountError,
                $"֧�� (ID: {payId}) ������Ԥ��: {expectedAmount}��ʵ��: {actualAmount}");
        }

        // �����˿�����쳣
        // refundId: �˿�ID
        // refundAmount: �˿���
        // payAmount: ԭ֧�����
        // ����: ҵ���쳣
        public static BusinessException RefundAmountExceeded(string refundId, decimal refundAmount, decimal payAmount)
        {
            return new BusinessException(ErrorCodes.RefundAmountExceeded,
                $"�˿� (ID: {refundId}) ��� ({refundAmount}) ����ԭ֧����� ({payAmount})");
        }
    }
}