import BaseCommonHttp from '../commonhttp'

export default class ClassService extends BaseCommonHttp {
  static controllerName='snclass'
  /**
   * @description 获取目录类别数据
   * @param {Object} playload
   */
  static async getDirectoryClass(playload) {
    // eslint-disable-next-line no-undef
    var { status, data } = await apiInstance({
      url: `/api/${this.controllerName}/get_directory_class?${this.getQuery(
        playload
      )}`,
      method: 'get'
    })
    return { status, data }
  }

  /**
   * @description 获取目录类别数据
   * @param {Object} playload
   */
  static async getInfo(playload) {
    // eslint-disable-next-line no-undef
    var { status, data } = await apiInstance({
      url: `/api/${this.controllerName}/getinfo/${playload}`,
      method: 'get'
    })
    return { status, data }
  }
}
