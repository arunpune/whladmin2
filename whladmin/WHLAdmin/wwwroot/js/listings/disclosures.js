$(function () {

  $('.whl-action-add-disclosure').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(listingDisclosureAddUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-disclosure-action').html('Add Disclosure');
        $('#hidDisclosureAction').val('ADD');
        $('#disclosureEditorModal').find('div.modal-body').html(response);
        $('#disclosureEditorModal').modal('show');
        $('#lblDisclosureSaveErrorMessage').html('');
        $('#divDisclosureSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-disclosure').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const disclosureId = $(e.currentTarget).data('disclosureid');
    $.get(listingDisclosureEditUrl + '?disclosureId=' + disclosureId)
      .done(function (response) {
        $('#whl-title-disclosure-action').html('Edit Disclosure');
        $('#hidDisclosureAction').val('EDIT');
        $('#disclosureEditorModal').find('div.modal-body').html(response);
        $('#disclosureEditorModal').modal('show');
        $('#lblDisclosureSaveErrorMessage').html('');
        $('#divDisclosureSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-disclosure').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#lblDisclosureSaveErrorMessage').html('');
    $('#divDisclosureSaveErrorMessage').hide();
    const a = $('#hidDisclosureAction').val();
    const url = a === 'EDIT' ? listingDisclosureEditUrl : listingDisclosureAddUrl;
    const data = {
      DisclosureId: $('#DisclosureId').val(),
      ListingId: $('#DisclosureListingId').val(),
      Text: $('#Text').val(),
      SortOrder: $('#SortOrder').val(),
      Active: $('#DisclosureActive').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#disclosureEditorModal').find('div.modal-body').html('');
        $('#disclosureEditorModal').modal('hide');
        const listingId = $('#PageListingId').val();
        fnShowToast('SUCCESS', 'Disclosures for Listing #' + listingId + ' updated, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save disclosure';
        $('#lblDisclosureSaveErrorMessage').html(e.responseJSON.message);
        $('#divDisclosureSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-disclosure').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const disclosureId = $(e.currentTarget).data('disclosureid');
    const disclosureText = $('#txtDisclosureText_' + disclosureId).val();
    if (confirm('Are you sure you want to delete the following disclosure?\n\n' + disclosureText)) {
      $.post(listingDisclosureDeleteUrl + '?disclosureId=' + disclosureId)
        .done(function (response) {
          const listingId = $('#PageListingId').val();
          fnShowToast('SUCCESS', 'Disclosure ' + disclosureId + ' for Listing #' + listingId + ' deleted, refreshing page.');
        });
    }
  });

});