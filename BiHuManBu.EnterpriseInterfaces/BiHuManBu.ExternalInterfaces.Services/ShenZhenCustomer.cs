using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Services
{
    class ShenZhenCustomer
    {
     public   string zhumadianjson = @"[{
	'module_code': 'customer_module',
	'text': '客户管理',
	'pater_code': 'system_all',
	'roleId': -1,
	'status': 'add',
	'orderby': 0,
	'buttonId': 0,
	'attrs': [{
		'module_code': 'customer_module',
		'pater_code': 'system_all',
		'roleId': -1,
		'status': '',
		'orderby': 0,
		'buttonId': 0
	}],
	'nodes': [{
		'module_code': 'addQuotedPrice',
		'text': '新增报价',
		'pater_code': 'customer_module',
		'roleId': -1,
		'status': 'add',
		'state': {
			'checked': true,
			'disabled': false,
			'expanded': false,
			'selected': false
		},
		'orderby': 0,
		'buttonId': 0,
		'attrs': [{
			'module_code': 'addQuotedPrice',
			'pater_code': 'customer_module',
			'roleId': -1,
			'status': '',
			'orderby': 0,
			'buttonId': 0
		}],
		'nodes': [],
		'nodeId': 841,
		'parentId': 840,
		'selectable': true
	}, {
		'module_code': 'customer_list',
		'text': '客户列表',
		'pater_code': 'customer_module',
		'roleId': -1,
		'status': 'add',
		'state': {
			'checked': true,
			'disabled': false,
			'expanded': true,
			'selected': false
		},
		'orderby': 0,
		'buttonId': 0,
		'attrs': [{
			'module_code': 'customer_list',
			'pater_code': 'customer_module',
			'roleId': -1,
			'status': '',
			'orderby': 0,
			'buttonId': 0
		}],
		'nodes': [{
			
			'module_code': 'btn_review',
			'text': '录入跟进结果',
			'pater_code': 'customer_list',
			'roleId': -1,
			'status': 'add',
			'orderby': 0,
			'buttonId': 4,
			'buttonCode': 'btn_review',
			'attrs': [{
				'module_code': 'btn_review',
				'text': '录入跟进结果',
				'pater_code': 'customer_list',
				'roleId': 0,
				'status': '',
				'orderby': 0,
				'buttonId': 4,
				'buttonCode': 'btn_review'
			}],
			'nodeId': 846,
			'parentId': 842,
			'selectable': true,
			'state': {
				'checked': true,
				'disabled': false,
				'expanded': false,
				'selected': false
			}
		}, {
			'module_code': 'btn_mass_edit',
			'text': '批量修改数据',
			'pater_code': 'customer_list',
			'roleId': -1,
			'status': 'add',
			'orderby': 0,
			'buttonId': 7,
			'buttonCode': 'btn_mass_edit',
			'attrs': [{
				'module_code': 'btn_mass_edit',
				'text': '批量修改数据',
				'pater_code': 'customer_list',
				'roleId': 0,
				'status': '',
				'orderby': 0,
				'buttonId': 7,
				'buttonCode': 'btn_mass_edit'
			}],
			'nodeId': 847,
			'parentId': 842,
			'selectable': true,
			'state': {
				'checked': true,
				'disabled': false,
				'expanded': false,
				'selected': false
			}
		}],
		'nodeId': 842,
		'parentId': 840,
		'selectable': true
	},  {
		'module_code': 'camera_list',
		'text': '摄像头进店',
		'pater_code': 'customer_module',
		'roleId': -1,
		'status': 'add',
		'state': {
			'checked': true,
			'disabled': false,
			'expanded': false,
			'selected': false
		},
		'orderby': 0,
		'buttonId': 0,
		'attrs': [{
			'module_code': 'camera_list',
			'pater_code': 'customer_module',
			'roleId': -1,
			'status': '',
			'orderby': 0,
			'buttonId': 0
		}],
		'nodes': [],
		'nodeId': 849,
		'parentId': 840,
		'selectable': true
	}, {
		'module_code': 'order_list',
		'text': '订单列表',
		'pater_code': 'customer_module',
		'roleId': -1,
		'status': 'add',
		'state': {
			'checked': true,
			'disabled': false,
			'expanded': false,
			'selected': false
		},
		'orderby': 0,
		'buttonId': 0,
		'attrs': [{
			'module_code': 'order_list',
			'pater_code': 'customer_module',
			'roleId': -1,
			'status': '',
			'orderby': 0,
			'buttonId': 0
		}],
		'nodes': [],
		'nodeId': 850,
		'parentId': 840,
		'selectable': true
	},  {
		'module_code': 'QuotationReceipt_List',
		'text': '已出保单',
		'pater_code': 'customer_module',
		'roleId': -1,
		'status': 'add',
		'state': {
			'checked': true,
			'disabled': false,
			'expanded': false,
			'selected': false
		},
		'orderby': 0,
		'buttonId': 0,
		'attrs': [{
			'module_code': 'QuotationReceipt_List',
			'pater_code': 'customer_module',
			'roleId': -1,
			'status': '',
			'orderby': 0,
			'buttonId': 0
		}],
		'nodes': [],
		'nodeId': 852,
		'parentId': 840,
		'selectable': true
	}, {
		'module_code': 'defeatReasonHistory',
		'text': '战败列表',
		'pater_code': 'customer_module',
		'roleId': -1,
		'status': 'add',
		'state': {
			'checked': true,
			'disabled': false,
			'expanded': false,
			'selected': false
		},
		'orderby': 0,
		'buttonId': 0,
		'attrs': [{
			'module_code': 'defeatReasonHistory',
			'pater_code': 'customer_module',
			'roleId': -1,
			'status': '',
			'orderby': 0,
			'buttonId': 0
		}],
		'nodes': [],
		'nodeId': 853,
		'parentId': 840,
		'selectable': true
	}, {
		'module_code': 'customer_recyclelist',
		'text': '回收站',
		'pater_code': 'customer_module',
		'roleId': -1,
		'status': 'add',
		'state': {
			'checked': true,
			'disabled': false,
			'expanded': false,
			'selected': false
		},
		'orderby': 0,
		'buttonId': 0,
		'attrs': [{
			'module_code': 'customer_recyclelist',
			'pater_code': 'customer_module',
			'roleId': -1,
			'status': '',
			'orderby': 0,
			'buttonId': 0
		}],
		'nodes': [],
		'nodeId': 854,
		'parentId': 840,
		'selectable': true
	}],
	'nodeId': 840,
	'selectable': true,
	'state': {
		'checked': true,
		'disabled': false,
		'expanded': true,
		'selected': false
	}

}]";

    }
}
