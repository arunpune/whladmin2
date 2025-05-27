$(function () {

  $('.whl-action-edit-petsallowed').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var listingId = $(e.currentTarget).data('listingid');
    $.get(listingPetsAllowedEditUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-petsallowed-action').html('Edit Pet Policy');
        $('#hidPetsAllowedAction').val('EDIT');
        $('#petsAllowedEditorModal').find('div.modal-body').html(response);
        $('#petsAllowedEditorModal').modal('show');
        $('#lblPetsAllowedSaveErrorMessage').html('');
        $('#divPetsAllowedSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-petsallowed').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#lblPetsAllowedSaveErrorMessage').html('');
    $('#divPetsAllowedSaveErrorMessage').hide();

    var a = $('#hidPetsAllowedAction').val();
    var url = listingPetsAllowedEditUrl;
    var data = {
      ListingId: $('#PetsAllowedListingId').val(),
      PetsAllowedInd: $('#PetsAllowedInd').prop('checked'),
      PetsAllowedText: $('#PetsAllowedText').val()
    };
    $.post(url, data)
      .done(function (response) {
        $('#petsAllowedEditorModal').find('div.modal-body').html('');
        $('#petsAllowedEditorModal').modal('hide');
        var listingId = $('#PageListingId').val();
        fnShowToast('SUCCESS', 'Pet Policy indicator for Listing #' + listingId + ' updated, refreshing page.');
      })
      .fail(function (e) {
        var message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save pets allowed indicator';
        $('#lblPetsAllowedSaveErrorMessage').html(e.responseJSON.message);
        $('#divPetsAllowedSaveErrorMessage').show();
      });
  });

});