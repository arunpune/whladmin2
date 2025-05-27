$(function () {

  $('.whl-action-add-documenttype').on('click', function (e) {
    e.preventDefault();
    $.get(documentTypeAddUrl)
      .done(function (response) {
        $('#whl-title-documenttype-action').html('Add Document Type');
        $('#hidDocumentTypeAction').val('ADD');
        $('#documentTypeEditorModal').find('div.modal-body').html(response);
        $('#documentTypeEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-documenttype').on('click', function (e) {
    e.preventDefault();
    const documentTypeId = $(e.currentTarget).data('documenttypeid');
    $.get(documentTypeEditUrl + '?documentTypeId=' + documentTypeId)
      .done(function (response) {
        $('#whl-title-documenttype-action').html('Edit Document Type');
        $('#hidDocumentTypeAction').val('EDIT');
        $('#documentTypeEditorModal').find('div.modal-body').html(response);
        $('#documentTypeEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-documenttype').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    const a = $('#hidDocumentTypeAction').val();
    const documentTypeName = $('#DocumentTypeName').val();
    const url = a === 'EDIT' ? documentTypeEditUrl : documentTypeAddUrl;
    const data = {
      DocumentTypeId: $('#DocumentTypeId').val(),
      DocumentTypeName: $('#DocumentTypeName').val(),
      DocumentTypeDescription: $('#DocumentTypeDescription').val(),
      Active: $('#Active').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#documentTypeEditorModal').find('div.modal-body').html('');
        $('#documentTypeEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'Document Type ' + documentTypeName + ' saved, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save documentType information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-documenttype').on('click', function (e) {
    e.preventDefault();
    const documentTypeId = $(e.currentTarget).data('documenttypeid');
    const documentTypeName = $(e.currentTarget).data('documentTypename');
    if (confirm('Are you sure you want to delete documentType - ' + documentTypeName + '?')) {
      $.post(documentTypeDeleteUrl + '?documentTypeId=' + documentTypeId)
        .done(function (response) {
          fnShowToast('SUCCESS', 'Document Type ' + documentTypeName + ' deleted, refreshing page.');
        });
    }
  });

});