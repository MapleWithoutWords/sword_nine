import BaseCommonHttp from '../commonhttp'

export default class DataSourceService extends BaseCommonHttp {
  static controllerName = 'sndatasource'

  static async getTypeDataList() {
    // eslint-disable-next-line no-undef
    var res = await apiInstance({
      url: `/api/${this.controllerName}/get_source_type`,
      method: 'get'
    })
    return res
  }

  static async generatorCode(playload) {
    // eslint-disable-next-line no-undef
    var res = await apiInstance({
      url: `/api/${this.controllerName}/generator_code/${playload.dataSourceId}/${playload.templateId}`,
      method: 'get'
    })
    return res
  }

  static async publish(dataSourceId) {
    // eslint-disable-next-line no-undef
    var res = await apiInstance({
      url: `/api/${this.controllerName}/publish/${dataSourceId}`,
      method: 'get'
    })
    return res
  }
}
