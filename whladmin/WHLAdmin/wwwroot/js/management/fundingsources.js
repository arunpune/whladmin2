$(function () {

  $('.whl-action-add-fundingsource').on('click', function (e) {
    e.preventDefault();
    $.get(fundingSourceAddUrl)
      .done(function (response) {
        $('#whl-title-fundingsource-action').html('Add Funding Source');
        $('#hidFundingSourceAction').val('ADD');
        $('#fundingSourceEditorModal').find('div.modal-body').html(response);
        $('#fundingSourceEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-fundingsource').on('click', function (e) {
    e.preventDefault();
    const fundingSourceId = $(e.currentTarget).data('fundingsourceid');
    $.get(fundingSourceEditUrl + '?fundingSourceId=' + fundingSourceId)
      .done(function (response) {
        $('#whl-title-fundingsource-action').html('Edit Funding Source');
        $('#hidFundingSourceAction').val('EDIT');
        $('#fundingSourceEditorModal').find('div.modal-body').html(response);
        $('#fundingSourceEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-fundingsource').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    const a = $('#hidFundingSourceAction').val();
    const fundingSourceName = $('#FundingSourceName').val();
    const url = a === 'EDIT' ? fundingSourceEditUrl : fundingSourceAddUrl;
    const data = {
      FundingSourceId: $('#FundingSourceId').val(),
      FundingSourceName: $('#FundingSourceName').val(),
      FundingSourceDescription: $('#FundingSourceDescription').val(),
      Active: $('#Active').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#fundingSourceEditorModal').find('div.modal-body').html('');
        $('#fundingSourceEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'Funding Source ' + fundingSourceName + ' saved, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save fundingSource information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-fundingsource').on('click', function (e) {
    e.preventDefault();
    const fundingSourceId = $(e.currentTarget).data('fundingsourceid');
    const fundingSourceName = $(e.currentTarget).data('fundingSourcename');
    if (confirm('Are you sure you want to delete fundingSource - ' + fundingSourceName + '?')) {
      $.post(fundingSourceDeleteUrl + '?fundingSourceId=' + fundingSourceId)
        .done(function (response) {
          fnShowToast('SUCCESS', 'Funding Source ' + fundingSourceName + ' deleted, refreshing page.');
        });
    }
  });

});