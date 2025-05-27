$(function () {

  $('.whl-action-add-resource').on('click', function (e) {
    e.preventDefault();
    $.get(resourceConfigAddUrl)
      .done(function (response) {
        $('#whl-title-resource-action').html('Add Resource');
        $('#hidResourceAction').val('ADD');
        $('#resourceEditorModal').find('div.modal-body').html(response);
        $('#resourceEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-resource').on('click', function (e) {
    e.preventDefault();
    const resourceId = $(e.currentTarget).data('resourceid');
    $.get(resourceConfigEditUrl + '?resourceId=' + resourceId)
      .done(function (response) {
        $('#whl-title-resource-action').html('Edit Resource');
        $('#hidResourceAction').val('EDIT');
        $('#resourceEditorModal').find('div.modal-body').html(response);
        $('#resourceEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-resource').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    const a = $('#hidResourceAction').val();
    const resourceTitle = $('#Title').val();
    const url = a === 'EDIT' ? resourceConfigEditUrl : resourceConfigAddUrl;
    const data = {
      ResourceId: $('#ResourceId').val(),
      Title: $('#Title').val(),
      Text: $('#Text').val(),
      Url: $('#Url').val(),
      DisplayOrder: $('#DisplayOrder').val(),
      Active: $('#Active').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#resourceEditorModal').find('div.modal-body').html('');
        $('#resourceEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'Resource ' + resourceTitle + ' saved, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save resource information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-resource').on('click', function (e) {
    e.preventDefault();
    const resourceId = $(e.currentTarget).data('resourceid');
    const resourceTitle = $(e.currentTarget).data('resourcetitle');
    if (confirm('Are you sure you want to delete Resource - ' + resourceTitle + '?')) {
      $.post(resourceConfigDeleteUrl + '?resourceId=' + resourceId)
        .done(function (response) {
          fnShowToast('SUCCESS', 'Resource ' + resourceTitle + ' deleted, refreshing page.');
        });
    }
  });

});