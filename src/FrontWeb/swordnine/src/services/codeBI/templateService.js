import BaseCommonHttp from '../commonhttp'

export default class TemplateService extends BaseCommonHttp {
  static controllerName = 'sntemplate'

  /**
   * @description 上传文件
   * @param {FormData} playload
   */
  static async upload(playload) {
    var config = { 'Content-Type': 'multipart/form-data' }
    // eslint-disable-next-line no-undef
    return await apiInstance.post(`/api/${this.controllerName}/upload_template`, playload, config)
    // var res = await apiInstance({
    //   url: `/api/${this.controllerName}/upload_template`,
    //   method: 'post',
    //   processData: false,
    //   contentType: false,
    //   data: playload
    // })
    // return res
  }
}
