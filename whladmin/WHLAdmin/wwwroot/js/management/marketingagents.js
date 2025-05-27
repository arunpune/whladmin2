$(function () {

  $('.whl-action-add-agent').on('click', function (e) {
    e.preventDefault();
    $.get(marketingAgentAddUrl)
      .done(function (response) {
        $('#whl-title-agent-action').html('Add Marketing Agent');
        $('#hidAgentAction').val('ADD');
        $('#agentEditorModal').find('div.modal-body').html(response);
        $('#agentEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-agent').on('click', function (e) {
    e.preventDefault();
    var agentId = $(e.currentTarget).data('agentid');
    $.get(marketingAgentEditUrl + '?agentId=' + agentId)
      .done(function (response) {
        $('#whl-title-agent-action').html('Edit Marketing Agent');
        $('#hidAgentAction').val('EDIT');
        $('#agentEditorModal').find('div.modal-body').html(response);
        $('#agentEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-agent').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    var a = $('#hidAgentAction').val();
    var agentName = $('#AgentName').val();
    var url = a === 'EDIT' ? marketingAgentEditUrl : marketingAgentAddUrl;
    var data = {
      AgentId: $('#AgentId').val(),
      AgentName: $('#AgentName').val(),
      ContactName: $('#ContactName').val(),
      PhoneNumber: $('#PhoneNumber').val(),
      EmailAddress: $('#EmailAddress').val(),
      Active: $('#Active').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#agentEditorModal').find('div.modal-body').html('');
        $('#agentEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'Agent ' + agentName + ' saved, refreshing page.');
      })
      .fail(function (e) {
        var message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save marketing agent information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-agent').on('click', function (e) {
    e.preventDefault();
    var agentId = $(e.currentTarget).data('agentid');
    var agentName = $(e.currentTarget).data('agentname');
    if (confirm('Are you sure you want to delete marketing agent - ' + agentName + '?')) {
      $.post(marketingAgentDeleteUrl + '?agentId=' + agentId)
        .done(function (response) {
          fnShowToast('SUCCESS', 'Marketing Agent ' + agentName + ' deleted, refreshing page.');
        });
    }
  });

});
