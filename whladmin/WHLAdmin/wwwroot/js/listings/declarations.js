$(function () {

  $('.whl-action-add-declaration').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(listingDeclarationAddUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-declaration-action').html('Add Declaration');
        $('#hidDeclarationAction').val('ADD');
        $('#declarationEditorModal').find('div.modal-body').html(response);
        $('#declarationEditorModal').modal('show');
        $('#lblDeclarationSaveErrorMessage').html('');
        $('#divDeclarationSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-declaration').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const declarationId = $(e.currentTarget).data('declarationid');
    $.get(listingDeclarationEditUrl + '?declarationId=' + declarationId)
      .done(function (response) {
        $('#whl-title-declaration-action').html('Edit Declaration');
        $('#hidDeclarationAction').val('EDIT');
        $('#declarationEditorModal').find('div.modal-body').html(response);
        $('#declarationEditorModal').modal('show');
        $('#lblDeclarationSaveErrorMessage').html('');
        $('#divDeclarationSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-declaration').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#lblDeclarationSaveErrorMessage').html('');
    $('#divDeclarationSaveErrorMessage').hide();
    const a = $('#hidDeclarationAction').val();
    const url = a === 'EDIT' ? listingDeclarationEditUrl : listingDeclarationAddUrl;
    const data = {
      DeclarationId: $('#DeclarationId').val(),
      ListingId: $('#DeclarationListingId').val(),
      Text: $('#Text').val(),
      SortOrder: $('#SortOrder').val(),
      Active: $('#DeclarationActive').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#declarationEditorModal').find('div.modal-body').html('');
        $('#declarationEditorModal').modal('hide');
        const listingId = $('#PageListingId').val();
        fnShowToast('SUCCESS', 'Declarations for Listing #' + listingId + ' updated, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save declaration';
        $('#lblDeclarationSaveErrorMessage').html(e.responseJSON.message);
        $('#divDeclarationSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-declaration').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const declarationId = $(e.currentTarget).data('declarationid');
    const declarationText = $('#txtDeclarationText_' + declarationId).val();
    if (confirm('Are you sure you want to delete the following declaration?\n\n' + declarationText)) {
      $.post(listingDeclarationDeleteUrl + '?declarationId=' + declarationId)
        .done(function (response) {
          const listingId = $('#PageListingId').val();
          fnShowToast('SUCCESS', 'Declaration ' + declarationId + ' for Listing #' + listingId + ' deleted, refreshing page.');
        });
    }
  });

});