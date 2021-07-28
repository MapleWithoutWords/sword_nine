import BaseCommonHttp from '../commonhttp'

export default class RuleAttributeSettingService extends BaseCommonHttp {
  static controllerName = 'snruleclassattributesetting'

  /**
   * @description 修改或新增
   * @param {Object} playload
   */
  static async updateOrInsert(playload) {
    // eslint-disable-next-line no-undef
    var res = await apiInstance({
      url: `/api/${this.controllerName}/update_or_insert`,
      method: 'post',
      data: playload
    })
    return res
  }

  /**
   * @description 修改或新增
   * @param {Object} playload
   */
  static async queryAttrRuleSetting(playload) {
    // eslint-disable-next-line no-undef
    var res = await apiInstance({
      url: `/api/${this.controllerName}/query_attr_rule_setting?${this.getQuery(playload)}`,
      method: 'get'
    })
    return res
  }
}
