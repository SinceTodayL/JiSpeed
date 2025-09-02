/**
 * 骑手API测试文件
 * 用于验证API接口的正确性和类型定义
 */

import {
  // 基础骑手管理
  getRiderInfo,
  getRiderOrderList,
  getRiderSchedule,
  getRiderAssignDetail,
  registerRider,
  createAttendanceRecord,
  updateRiderInfo,
  updateAssignStatus,
  updateAttendanceRecord,

  // 位置管理
  getRiderLatestLocation,
  getRiderLocationHistory,
  getRidersInArea,
  getOnlineRidersLocation,
  calculateRiderDistance,
  getRiderCurrentAddress,
  updateRiderLocation,
  updateRiderOnlineStatus,

  // 绩效管理
  getRiderPerformance,
  getRiderPerformanceTrend,
  getRiderPerformanceRanking,
  getTopPerformers,
  getMonthlyPerformanceOverview,
  generateRiderPerformance
} from './src/service/api';

// 测试数据
const testRiderId = 'rider001';
const testYear = 2025;
const testMonth = 1;

// 测试基础骑手管理API
async function testBasicRiderAPIs() {
  console.log('=== 测试基础骑手管理API ===');

  try {
    // 1. 获取骑手信息
    console.log('1. 测试获取骑手信息');
    const riderInfo = await getRiderInfo({ riderId: testRiderId });
    console.log('骑手信息:', riderInfo);

    // 2. 获取骑手订单列表
    console.log('2. 测试获取骑手订单列表');
    const orders = await getRiderOrderList(testRiderId, { status: 1 });
    console.log('订单列表:', orders);

    // 3. 获取骑手排班
    console.log('3. 测试获取骑手排班');
    const schedules = await getRiderSchedule(testRiderId, {
      startDate: '2025-01-01',
      endDate: '2025-01-31'
    });
    console.log('排班信息:', schedules);

    // 4. 获取订单分配详情
    console.log('4. 测试获取订单分配详情');
    const assignDetail = await getRiderAssignDetail({
      riderId: testRiderId,
      assignId: 'assign001'
    });
    console.log('分配详情:', assignDetail);

  } catch (error) {
    console.error('基础骑手管理API测试失败:', error);
  }
}

// 测试位置管理API
async function testLocationAPIs() {
  console.log('\n=== 测试位置管理API ===');

  try {
    // 1. 获取骑手最新位置
    console.log('1. 测试获取骑手最新位置');
    const latestLocation = await getRiderLatestLocation(testRiderId);
    console.log('最新位置:', latestLocation);

    // 2. 获取骑手历史轨迹
    console.log('2. 测试获取骑手历史轨迹');
    const history = await getRiderLocationHistory({
      riderId: testRiderId,
      startTime: '2025-01-20T00:00:00',
      endTime: '2025-01-20T23:59:59',
      pageIndex: 1,
      pageSize: 10
    });
    console.log('历史轨迹:', history);

    // 3. 获取区域内的骑手
    console.log('3. 测试获取区域内的骑手');
    const areaRiders = await getRidersInArea({
      minLongitude: 116.0,
      maxLongitude: 117.0,
      minLatitude: 39.0,
      maxLatitude: 40.0,
      status: 1
    });
    console.log('区域内骑手:', areaRiders);

    // 4. 获取在线骑手位置
    console.log('4. 测试获取在线骑手位置');
    const onlineRiders = await getOnlineRidersLocation({
      pageIndex: 1,
      pageSize: 20
    });
    console.log('在线骑手:', onlineRiders);

    // 5. 计算距离
    console.log('5. 测试计算距离');
    const distance = await calculateRiderDistance({
      riderId: testRiderId,
      targetLongitude: 116.41,
      targetLatitude: 39.9
    });
    console.log('距离:', distance);

    // 6. 获取地址信息
    console.log('6. 测试获取地址信息');
    const address = await getRiderCurrentAddress(testRiderId);
    console.log('地址信息:', address);

  } catch (error) {
    console.error('位置管理API测试失败:', error);
  }
}

// 测试绩效管理API
async function testPerformanceAPIs() {
  console.log('\n=== 测试绩效管理API ===');

  try {
    // 1. 获取骑手月度绩效
    console.log('1. 测试获取骑手月度绩效');
    const performance = await getRiderPerformance({
      riderId: testRiderId,
      year: testYear,
      month: testMonth
    });
    console.log('月度绩效:', performance);

    // 2. 获取绩效趋势
    console.log('2. 测试获取绩效趋势');
    const trend = await getRiderPerformanceTrend({
      riderId: testRiderId,
      months: 6
    });
    console.log('绩效趋势:', trend);

    // 3. 获取绩效排名
    console.log('3. 测试获取绩效排名');
    const ranking = await getRiderPerformanceRanking({
      riderId: testRiderId,
      year: testYear,
      month: testMonth
    });
    console.log('绩效排名:', ranking);

    // 4. 获取优秀骑手排行榜
    console.log('4. 测试获取优秀骑手排行榜');
    const topPerformers = await getTopPerformers({
      year: testYear,
      month: testMonth,
      topCount: 10
    });
    console.log('优秀骑手:', topPerformers);

    // 5. 获取月度绩效概览
    console.log('5. 测试获取月度绩效概览');
    const overview = await getMonthlyPerformanceOverview({
      year: testYear,
      month: testMonth
    });
    console.log('绩效概览:', overview);

  } catch (error) {
    console.error('绩效管理API测试失败:', error);
  }
}

// 测试POST和PATCH API
async function testModificationAPIs() {
  console.log('\n=== 测试修改类API ===');

  try {
    // 1. 更新骑手位置
    console.log('1. 测试更新骑手位置');
    const locationUpdate = await updateRiderLocation({
      riderId: testRiderId,
      longitude: 116.407526,
      latitude: 39.9042,
      accuracy: 10.5,
      speed: 25.3,
      direction: 180,
      locationType: 'GPS'
    });
    console.log('位置更新:', locationUpdate);

    // 2. 更新骑手在线状态
    console.log('2. 测试更新骑手在线状态');
    const statusUpdate = await updateRiderOnlineStatus(testRiderId, { status: 1 });
    console.log('状态更新:', statusUpdate);

    // 3. 生成月度绩效
    console.log('3. 测试生成月度绩效');
    const generatedPerformance = await generateRiderPerformance({
      riderId: testRiderId,
      year: testYear,
      month: testMonth
    });
    console.log('生成的绩效:', generatedPerformance);

  } catch (error) {
    console.error('修改类API测试失败:', error);
  }
}

// 主测试函数
async function runAllTests() {
  console.log('开始运行骑手API测试...\n');

  try {
    await testBasicRiderAPIs();
    await testLocationAPIs();
    await testPerformanceAPIs();
    await testModificationAPIs();

    console.log('\n=== 所有测试完成 ===');
  } catch (error) {
    console.error('测试过程中发生错误:', error);
  }
}

// 如果直接运行此文件，则执行测试
if (require.main === module) {
  runAllTests();
}

export {
  testBasicRiderAPIs,
  testLocationAPIs,
  testPerformanceAPIs,
  testModificationAPIs,
  runAllTests
};
