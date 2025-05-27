$(function () {

  $('.whl-action-add-amenity').on('click', function (e) {
    e.preventDefault();
    $.get(amenityAddUrl)
      .done(function (response) {
        $('#whl-title-amenity-action').html('Add Amenity');
        $('#hidAmenityAction').val('ADD');
        $('#amenityEditorModal').find('div.modal-body').html(response);
        $('#amenityEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-amenity').on('click', function (e) {
    e.preventDefault();
    const amenityId = $(e.currentTarget).data('amenityid');
    $.get(amenityEditUrl + '?amenityId=' + amenityId)
      .done(function (response) {
        $('#whl-title-amenity-action').html('Edit Amenity');
        $('#hidAmenityAction').val('EDIT');
        $('#amenityEditorModal').find('div.modal-body').html(response);
        $('#amenityEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-amenity').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    const a = $('#hidAmenityAction').val();
    const amenityName = $('#AmenityName').val();
    const url = a === 'EDIT' ? amenityEditUrl : amenityAddUrl;
    const data = {
      AmenityId: $('#AmenityId').val(),
      AmenityName: $('#AmenityName').val(),
      AmenityDescription: $('#AmenityDescription').val(),
      Active: $('#Active').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#amenityEditorModal').find('div.modal-body').html('');
        $('#amenityEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'Amenity ' + amenityName + ' saved, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save amenity information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-amenity').on('click', function (e) {
    e.preventDefault();
    const amenityId = $(e.currentTarget).data('amenityid');
    const amenityName = $(e.currentTarget).data('amenityname');
    if (confirm('Are you sure you want to delete amenity - ' + amenityName + '?')) {
      $.post(amenityDeleteUrl + '?amenityId=' + amenityId)
        .done(function (response) {
          fnShowToast('SUCCESS', 'Amenity ' + amenityName + ' deleted, refreshing page.');
        });
    }
  });

});