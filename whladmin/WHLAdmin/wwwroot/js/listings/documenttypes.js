$(function () {

  $('.whl-action-edit-documentsreqd').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(listingDocumentTypesEditUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-documentsreqd-action').html(documentTypesCount > 0 ? 'Edit Document Types' : 'Add Document Types');
        $('#hidDocumentsReqdAction').val('EDIT');
        $('#documentsReqdEditorModal').find('div.modal-body').html(response);
        $('#documentsReqdEditorModal').modal('show');
        $('#lblDocumentsReqdSaveErrorMessage').html('');
        $('#divDocumentsReqdSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-documentsreqd').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#lblDocumentsReqdSaveErrorMessage').html('');
    $('#divDocumentsReqdSaveErrorMessage').hide();

    let selectedDocumentTypeIds = '';
    $("input:checkbox[name=selDocumentTypes]:checked").each(function () {
      selectedDocumentTypeIds += (selectedDocumentTypeIds.length > 0 ? ',' : '') + $(this).val();
    });

    const a = $('#hidDocumentsReqdAction').val();
    const url = a === 'EDIT' ? listingDocumentTypesEditUrl : listingDocumentTypesAddUrl;
    const data = {
      ListingId: $('#DocumentTypeListingId').val(),
      DocumentTypeIds: selectedDocumentTypeIds
    };
    $.post(url, data)
      .done(function (response) {
        $('#documentsReqdEditorModal').find('div.modal-body').html('');
        $('#documentsReqdEditorModal').modal('hide');
        const listingId = $('#PageListingId').val();
        fnShowToast('SUCCESS', 'Document Types for Listing #' + listingId + ' updated, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save amenities';
        $('#lblDocumentsReqdSaveErrorMessage').html(e.responseJSON.message);
        $('#divDocumentsReqdSaveErrorMessage').show();
      });
  });

});