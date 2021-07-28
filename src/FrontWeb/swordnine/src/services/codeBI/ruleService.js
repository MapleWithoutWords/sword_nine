import BaseCommonHttp from '../commonhttp'

export default class RuleService extends BaseCommonHttp {
  static controllerName = 'snrule'
  static async save(playload) {
    // eslint-disable-next-line no-undef
    var { status, data } = await apiInstance({
      url: `/api/${this.controllerName}/save`,
      method: 'post',
      data: playload
    })
    return { status, data }
  }
}
