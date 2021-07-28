import BaseCommonHttp from '../commonhttp'

export default class ClassAttributeService extends BaseCommonHttp {
  static controllerName = 'snclassattribute'
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
