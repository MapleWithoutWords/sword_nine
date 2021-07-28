// 绘图内容的双击事件，向父窗口发送当前选择的cell
window.sendSelectionCell = function () {
  var cell = window.editorUiInstance.editor.graph.getSelectionCell()
  if (cell.edge) {
    // TODO:选中的是线
    window.sendErrorMsg('线暂时无法编辑');
    return false
  }
  if (!window.parent) {
    console.log(cell)
    return false
  }
  if (cell.sntype === 1) {
    return false
  }
  if (!window.parent.handleCellClick) {
    console.log('parent window "handleCellClick" function is not defind')
    return false
  }
  // 向父窗口发送消息,发送当前选中的cell
  window.parent.handleCellClick(cell.clone())
}
// 绘图的保存事件，向父窗口发送整个绘图的内容
window.sendSaveData = function () {
  var xmlString = mxUtils.getPrettyXml(window.editorUiInstance.editor.getGraphXml())
  // var targetOrigin = location.origin
  if (!window.parent) {
    return false
  }
  if (!window.parent.handleSaveData) {
    console.log('parent window "handleSaveData" function is not defind')
    return false
  }
  // 向父窗口发送消息，发送保存的对象数据
  window.parent.handleSaveData(xmlString)
}

// 更新当前选中cell的信息
window.updateClassCell = function (data) {
  // 当前选中的cell
  var selectCell = window.editorUiInstance.editor.graph.getSelectionCell()
  if (!selectCell) {
    window.sendErrorMsg('请选择图元')
  }
  selectCell.setValue(data.value)
  selectCell.dataId = data.dataId
  selectCell.sntype = 0
  window.editorUiInstance.editor.graph.refresh(selectCell)
}
// 更新当前选中cell的子节点信息
window.updateAttributeCell = function (data) {
  // 当前选中的cell
  var selectCell = window.editorUiInstance.editor.graph.getSelectionCell()
  // 将数据写入到当前cell
  var graphInstance = window.editorUiInstance.editor.graph
  for (let index = 0; index < data.length; index++) {
    const element = data[index];
    var graphCell = selectCell.children.find(e => e.dataId === element.dataId)
    if (!graphCell) {
      var retCell = window.createChildCell(selectCell, selectCell.children.length * 30)
      window.setDataValue(retCell, element)
      graphInstance.addCell(retCell, selectCell, selectCell.children.length)
    } else {
      window.setDataValue(graphCell, element)
    }
  }
  if (selectCell.children.length > 1) {
    var delDatas = selectCell.children.filter(e => !e.dataId || data.findIndex(x => x.dataId === e.dataId) < 0)
    if (delDatas.length !== selectCell.children.length) {
      graphInstance.deleteCells(delDatas, true)
    }
  }
  window.editorUiInstance.editor.graph.refresh(selectCell)
}
// 重新渲染整个绘图
window.reRenderDrawing = function (data) {
  if (!data) {
    var delCellList = window.editorUiInstance.editor.graph.getModel().root.children[0].children
    if (delCellList && delCellList.length > 0) {
      window.editorUiInstance.editor.graph.removeCells(delCellList)
    }
    window.IsEditored = false
    return false
  }

  window.editorUiInstance.editor.graph.model.beginUpdate();
  try {
    window.editorUiInstance.editor.setGraphXml(mxUtils.parseXml(data).documentElement);
    // LATER: Why is hideDialog between begin-/endUpdate faster?
  }
  catch (e) {
    console.error(e)
    window.sendErrorMsg(e)
  }
  finally {
    window.editorUiInstance.editor.graph.model.endUpdate();
  }
  window.IsEditored = false
}

// 将数据转换成cell对象
window.convertToCells = function (data) {
  if (!data || data.length < 1) {
    return []
  }
  var ret = []
  var forChildTree = function (elData, parentCell, lineDataList, lineConnectCellList) {
    if (!elData || !elData.children || elData.children.length < 1) {
      return false
    }
    parentCell.children = parentCell.children || []
    for (let index = 0; index < elData.children.length; index++) {
      const element = elData.children[index];
      // eslint-disable-next-line new-cap
      var cell = new mxCell(null, null, null)
      cell.setData(element, parentCell)
      parentCell.children.push(cell)
      forChildTree(element, cell, lineDataList, lineConnectCellList)
      if (cell.edge === false && lineDataList.findIndex(e => (e.source && e.source.id === cell.id) || (e.target && e.target.id === cell.id)) > -1) {
        lineConnectCellList.push(cell)
      }
    }
  }
  // 所有是线的数据
  var lineDataList = data.filter(e => e.edge)
  // 所有线要连接的cell对象集合
  var lineConnectCellList = []
  var lineCellList = []
  for (var idx = 0; idx < data.length; idx++) {
    const element = data[idx]
    // eslint-disable-next-line new-cap
    var cell = new mxCell(null, null, null)
    cell.setData(element, null)
    if (cell.edge === false && lineDataList.findIndex(e => e.source.id === cell.id || e.target.id === cell.id) > -1) {
      lineConnectCellList.push(cell)
    }
    if (cell.edge) {
      lineCellList.push(cell)
    }
    ret.push(cell)
    forChildTree(element, cell, lineDataList, lineConnectCellList)
  }
  for (let index = 0; index < lineCellList.length; index++) {
    const element = lineCellList[index];
    if (element.source) {
      var sourceCell = lineConnectCellList.find(e => e.id === element.source.id)
      if (!sourceCell) {
        sourceCell = null
        console.error(`未找到连接点${element.source.id}`)
      }
      element.setTerminal(sourceCell, true)
    }
    if (element.target) {
      var targetCell = lineConnectCellList.find(e => e.id === element.target.id)
      if (!targetCell) {
        console.error(`未找到连接点${element.target.id}`)
        targetCell = null
      }
      element.setTerminal(targetCell, false)
    }
  }
  return ret
}

// 绘图的删除cell前执行的方法
window.deleteCells = function (cells) {
  if (cells.findIndex(e => e.sntype && e.sntype === 1) > -1) {
    return false
  }
  if (cells.findIndex(e => e.sntype === 0) > -1) {
    if (window.parent && window.parent.delClass) {
      window.parent.delClass(cells.map(e => e.dataId))
    }
  }
  return true
}

// 新增一个 table
window.quicklyAddClass = function (classData, attributeList) {
  var graphInstance = window.editorUiInstance.editor.graph
  var tableCell = window.createTable(classData, attributeList, 0, 0)
  var data = window.editorUiInstance.editor.graph.getModel().root.children[0].children
  if (data && data.length > 0) {
    if (data.findIndex(e => e.dataId === classData.dataId) > -1) {
      window.sendErrorMsg('当前图元已存在')
      return false
    }
    graphInstance.addCell(tableCell, data[0].parent, data[0].parent.children.length)
  } else {
    window.editorUiInstance.editor.graph.addCells([tableCell])
  }
}

// 创建一个表格
window.createTable = function (classData, attributeList, x = 130, y = 110, childrenWidthList = [30, 160, 100, 40]) {
  var tableCell
  // var data = window.editorUiInstance.editor.graph.getModel().root.children[0].children
  // if (data) {
  //   var tableList = data.filter(e => e.edge === false)
  //   if (tableList && tableList.length > 0) {
  //     tableCell = window.editorUiInstance.editor.graph.cloneCell(tableList[tableList.length - 1], true)
  //     tableCell.dataId = classData.dataId
  //     tableCell.value = classData.value
  //     tableCell.geometry.x = tableCell.geometry.x + 30
  //     tableCell.geometry.y = tableCell.geometry.y + 30
  //     tableCell.children = []
  //     for (let index = 0; index < attributeList.length; index++) {
  //       const element = attributeList[index]
  //       var retCell = window.createChildCell(tableCell, (index + 1) * 30)
  //       window.setDataValue(retCell, element)
  //       tableCell.children.push(retCell)
  //     }
  //     if (tableCell.children.length < 1) {
  //       var defaultCell = window.createChildCell(tableCell)
  //       tableCell.children.push(defaultCell)
  //     }
  //   }
  // }
  if (!tableCell) {
    var tableStyle = "shape=table;startSize=30;container=1;collapsible=1;childLayout=tableLayout;fontStyle=1;align=center;columnLines=0;fixedRows=1;rowLines=1;strokeWidth=0;fillColor=#f5f5f5;strokeColor=#666666;fontColor=#333333;resizeLast=0;resizeLastRow=0;fixedRows=1;";
    var defaultHeight = 30
    tableCell = new mxCell(classData.value, new mxGeometry(x, y, 330, 60), tableStyle)
    tableCell.dataId = classData.dataId
    tableCell.setVertex(1)
    tableCell.setEdge(false)
    tableCell.setConnectable(true)
    tableCell.setVisible(true)
    tableCell.setCollapsed(false)
    tableCell.children = []
    tableCell.source = null
    tableCell.target = null
    tableCell.sntype = 0
    var firstCellStyle = "shape=partialRectangle;html=1;whiteSpace=wrap;collapsible=0;dropTarget=0;pointerEvents=0;fillColor=none;top=0;left=0;bottom=0;right=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;";
    var childCellStyle = "shape=partialRectangle;html=1;whiteSpace=wrap;connectable=0;fillColor=none;top=0;left=0;bottom=0;right=0;overflow=hidden;align=left;spacingLeft=12;";
    var childX = 0; var childY = 0
    for (const key in attributeList) {
      const element = attributeList[key];
      var firstCell = new mxCell('', new mxGeometry(0, childY, tableCell.geometry.width, defaultHeight), firstCellStyle)
      firstCell.dataId = element.dataId
      firstCell.setVertex(1)
      firstCell.setEdge(false)
      firstCell.setConnectable(true)
      firstCell.setVisible(true)
      firstCell.setCollapsed(false)
      firstCell.children = []
      firstCell.setParent(tableCell)
      childX = 0
      for (const idx in element.valueList) {
        const val = element.valueList[idx];
        var addCell = new mxCell(val, new mxGeometry(childX, childY, childrenWidthList[idx], defaultHeight), childCellStyle)
        addCell.dataId = element.dataId
        addCell.setVertex(1)
        addCell.setEdge(false)
        addCell.setConnectable(true)
        addCell.setVisible(true)
        addCell.setCollapsed(false)
        addCell.setParent(firstCell)
        childX += childrenWidthList[idx]
        firstCell.children.push(addCell)
      }
      childY += defaultHeight
      tableCell.children.push(firstCell)
    }
  }
  return tableCell
}
// 创建一个子属性
window.createChildCell = function (cell, cellOffsetY = 30, len = 4, widthList = [30, 160, 100, 40]) {
  if (cell.children && cell.children.length > 0) {
    var ret = window.editorUiInstance.editor.graph.cloneCell(cell.children[cell.children.length - 1], true)
    ret.geometry.y = cellOffsetY
    return ret
  }

  var firstCell = new mxCell('', new mxGeometry(0, cellOffsetY, cell.geometry.width, 30), 'shape=partialRectangle;html=1;whiteSpace=wrap;collapsible=0;dropTarget=0;pointerEvents=0;fillColor=none;top=0;left=0;bottom=0;right=0;points=[[0,0.5],[1,0.5]];portConstraint=eastwest;')
  firstCell.children = []
  var defaultStyle = 'shape=partialRectangle;html=1;whiteSpace=wrap;connectable=0;fillColor=none;top=0;left=0;bottom=0;right=0;overflow=hidden;align=left;spacingLeft=12;'
  var offsetX = 0
  for (let index = 0; index < len; index++) {
    var cellWidth = widthList[widthList.length - 1]
    if (widthList.length > index) {
      cellWidth = widthList[index]
    }
    var newCell = new mxCell('', new mxGeometry(offsetX, 30, cellWidth, 30), defaultStyle)
    window.setCellDefaultValue(newCell)
    newCell.setParent(firstCell)
    firstCell.children.push(newCell)
    offsetX += cellWidth
  }
  // firstCell.setParent(cell)
  window.setCellDefaultValue(firstCell)
  return firstCell
}
// 设置cell子节点值
window.setDataValue = function (cell, data) {
  cell.dataId = data.dataId
  cell.sntype = 1
  if (cell.children && cell.children.length >= data.valueList.length) {
    for (const key in data.valueList) {
      if (Object.hasOwnProperty.call(data.valueList, key)) {
        const element = data.valueList[key];
        cell.children[key].dataId = data.dataId
        cell.children[key].value = element
        cell.children[key].sntype = 1
      }
    }
  }
  return cell
}
// 设置cell的默认值
window.setCellDefaultValue = function (cell) {
  cell.setVertex(1)
  if (cell.edge) {
    cell.setEdge(cell.edge)
  } else {
    cell.setEdge(false)
  }
  cell.setConnectable(true)
  cell.setVisible(true)
  cell.setCollapsed(false)
}
// 发送错误信息
window.sendErrorMsg = function (errorMsg) {
  if (window.parent && window.parent.receiveErrorMsg) {
    window.parent.receiveErrorMsg(errorMsg)
    return false
  }
  alert(errorMsg)
}
window.updateEditorStatus = function (flag) {
  window.IsEditored = flag
}
// 是否编辑过
window.IsEditored = false
