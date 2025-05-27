$(function () {

  $('.whl-action-add-document').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(listingDocumentAddUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-document-action').html('Add Document');
        $('#hidDocumentAction').val('ADD');
        $('#documentEditorModal').find('div.modal-body').html(response);
        $('#documentEditorModal').modal('show');
        $('#lblDocumentSaveErrorMessage').html('');
        $('#divDocumentSaveErrorMessage').hide();
        $('#documentFile').on('change', function (e) {
          if (fnIsValidFileSize('documentFile', 5)) {
            fnGetBase64('documentFile', 'DocumentContents', 'DocumentFileName');
          }
        });
      });
  });

  $('.whl-action-edit-document').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const documentId = $(e.currentTarget).data('documentid');
    $.get(listingDocumentEditUrl + '?documentId=' + documentId)
      .done(function (response) {
        $('#whl-title-document-action').html('Edit Document Properties');
        $('#hidDocumentAction').val('EDIT');
        $('#documentEditorModal').find('div.modal-body').html(response);
        $('#documentEditorModal').modal('show');
        $('#lblDocumentSaveErrorMessage').html('');
        $('#divDocumentSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-document').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#lblDocumentSaveErrorMessage').html('');
    $('#divDocumentSaveErrorMessage').hide();
    const a = $('#hidDocumentAction').val();
    const url = a === 'EDIT' ? listingDocumentEditUrl : listingDocumentAddUrl;
    const data = {
      DocumentId: $('#DocumentId').val(),
      ListingId: $('#DocumentListingId').val(),
      Title: $('#DocumentTitle').val(),
      FileName: $('#DocumentFileName').val(),
      Contents: $('#DocumentContents').val(),
      MimeType: $('#DocumentMimeType').val(),
      DisplayOnListingsPageInd: $('#DisplayOnListingsPageInd').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#documentEditorModal').find('div.modal-body').html('');
        $('#documentEditorModal').modal('hide');
        const listingId = $('#PageListingId').val();
        fnShowToast('SUCCESS', 'Documents for Listing #' + listingId + ' updated, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save document';
        $('#lblDocumentSaveErrorMessage').html(e.responseJSON.message);
        $('#divDocumentSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-document').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const documentId = $(e.currentTarget).data('documentid');
    if (confirm('Are you sure you want to delete listing document - ' + documentId + '?')) {
      $.post(listingDocumentDeleteUrl + '?documentId=' + documentId)
        .done(function (response) {
          const listingId = $('#PageListingId').val();
          fnShowToast('SUCCESS', 'Document ' + documentId + ' for Listing #' + listingId + ' deleted, refreshing page.');
        });
    }
  });

  $('.whl-action-view-document').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const documentId = $(e.currentTarget).data('documentid');
    const a = document.createElement('a');
    a.download = '';
    a.href = listingDocumentViewUrl + '?documentId=' + documentId;
    a.click();
  });

});