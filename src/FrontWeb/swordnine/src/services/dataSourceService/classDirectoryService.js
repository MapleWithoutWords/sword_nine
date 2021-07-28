import BaseCommonHttp from '../commonhttp'

export default class ClassDirectoryService extends BaseCommonHttp {
  static controllerName = 'snclassdirectory'
  static async getDirectoryTree(playload) {
    // eslint-disable-next-line no-undef
    var { status, data } = await apiInstance({
      url: `/api/${this.controllerName}/get_directory_tree?${this.getQuery(
        playload
      )}`,
      method: 'get'
    })
    return { status, data }
  }

  static async saveDrawing(playload) {
    // eslint-disable-next-line no-undef
    var { status, data } = await apiInstance({
      url: `/api/${this.controllerName}/save_drawing`,
      method: 'post',
      data: playload
    })
    return { status, data }
  }
}
